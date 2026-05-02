using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Models;
using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Services;

public interface ISheetService
{
    Task<List<SheetResponseDto>> GetAllAsync();
    Task<List<SheetResponseDto>> GetByMechanicAsync(int mechanicId);
    Task<SheetResponseDto?> GetByIdAsync(int id, int requesterId, string requesterRole);
    Task<SheetResponseDto> CreateAsync(CreateSheetDto dto, int createdById);
    Task<SheetResponseDto> UpdateAsync(int id, UpdateSheetDto dto, int requesterId);
    Task<SheetResponseDto> SubmitAsync(int id, int mechanicId);
    Task<SheetResponseDto> ApproveAsync(int id, int managerId);
    Task<SheetResponseDto> RejectAsync(int id, int managerId, string rejectionNote);
    Task<DefectItemResponseDto> AddDefectItemAsync(int sheetId, CreateDefectItemDto dto, int requesterId);
    Task DeleteDefectItemAsync(int sheetId, int itemId, int requesterId);
    SheetResponseDto MapToDto(Sheet s);
}

public class SheetService : ISheetService
{
    private readonly AppDbContext _db;

    public SheetService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SheetResponseDto>> GetAllAsync()
    {
        var sheets = await _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.ReviewedBy)
            .Include(s => s.Technicians).ThenInclude(t => t.User)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .ToListAsync();

        return sheets.Select(MapToDto).ToList();
    }

    public async Task<List<SheetResponseDto>> GetByMechanicAsync(int mechanicId)
    {
        var sheets = await _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.ReviewedBy)
            .Include(s => s.Technicians).ThenInclude(t => t.User)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .Where(s => s.CreatedById == mechanicId)
            .ToListAsync();

        return sheets.Select(MapToDto).ToList();
    }

    public async Task<SheetResponseDto?> GetByIdAsync(int id, int requesterId, string requesterRole)
    {
        var sheet = await GetSheetWithRelationsAsync(id);

        if (sheet == null) return null;

       
        if (requesterRole == "mechanic" && sheet.CreatedById != requesterId)
            return null;

        return MapToDto(sheet);
    }

    public async Task<SheetResponseDto> CreateAsync(CreateSheetDto dto, int createdById)
    {
        var sheet = new Sheet
        {
            Code = dto.Code,
            Brand = dto.Brand,
            Vehicle = dto.Vehicle,
            InspectionDate = dto.InspectionDate,
            SheetStatus = SheetStatusEnum.Draft,
            CreatedById = createdById
        };

        _db.Sheets.Add(sheet);
        await _db.SaveChangesAsync();

        if (dto.TechnicianIds.Any())
        {
            foreach (var techId in dto.TechnicianIds.Distinct())
            {
                _db.SheetTechnicians.Add(new SheetTechnician
                {
                    SheetId = sheet.Id,
                    UserId = techId
                });
            }
            await _db.SaveChangesAsync();
        }

        return MapToDto((await GetSheetWithRelationsAsync(sheet.Id))!);
    }

    public async Task<SheetResponseDto> UpdateAsync(int id, UpdateSheetDto dto, int requesterId)
    {
        var sheet = await _db.Sheets
            .Include(s => s.Technicians)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.CreatedById != requesterId)
            throw new UnauthorizedAccessException("Non autorizzato");

        if (sheet.SheetStatus != SheetStatusEnum.Draft && sheet.SheetStatus != SheetStatusEnum.Rejected)
            throw new InvalidOperationException($"Impossibile modificare una scheda in stato '{sheet.SheetStatus}'");

        if (dto.Code != null) sheet.Code = dto.Code;
        if (dto.Brand != null) sheet.Brand = dto.Brand;
        if (dto.Vehicle != null) sheet.Vehicle = dto.Vehicle;
        if (dto.InspectionDate.HasValue) sheet.InspectionDate = dto.InspectionDate.Value;
        sheet.UpdatedAt = DateTime.UtcNow;

        if (dto.TechnicianIds != null)
        {
            _db.SheetTechnicians.RemoveRange(sheet.Technicians);
            foreach (var techId in dto.TechnicianIds.Distinct())
            {
                _db.SheetTechnicians.Add(new SheetTechnician
                {
                    SheetId = sheet.Id,
                    UserId = techId
                });
            }
        }

        await _db.SaveChangesAsync();
        return MapToDto((await GetSheetWithRelationsAsync(id))!);
    }

    public async Task<SheetResponseDto> SubmitAsync(int id, int mechanicId)
    {
        var sheet = await _db.Sheets
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.CreatedById != mechanicId)
            throw new UnauthorizedAccessException("Non autorizzato");

        if (sheet.SheetStatus != SheetStatusEnum.Draft && sheet.SheetStatus != SheetStatusEnum.Rejected)
            throw new InvalidOperationException($"Impossibile sottomettere una scheda in stato '{sheet.SheetStatus}'");

        if (!sheet.DefectItems.Any())
            throw new InvalidOperationException("La scheda deve contenere almeno un difetto");

        sheet.SheetStatus = SheetStatusEnum.Submitted;
        sheet.SubmittedAt = DateTime.UtcNow;
        sheet.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto((await GetSheetWithRelationsAsync(id))!);
    }

    public async Task<SheetResponseDto> ApproveAsync(int id, int managerId)
    {
        var sheet = await _db.Sheets.FindAsync(id)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.SheetStatus != SheetStatusEnum.Submitted)
            throw new InvalidOperationException("Solo le schede submitted possono essere approvate");

        sheet.SheetStatus = SheetStatusEnum.Approved;
        sheet.ReviewedAt = DateTime.UtcNow;
        sheet.ReviewedById = managerId;
        sheet.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto((await GetSheetWithRelationsAsync(id))!);
    }

    public async Task<SheetResponseDto> RejectAsync(int id, int managerId, string rejectionNote)
    {
        var sheet = await _db.Sheets.FindAsync(id)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.SheetStatus != SheetStatusEnum.Submitted)
            throw new InvalidOperationException("Solo le schede submitted possono essere rifiutate");

       

        sheet.SheetStatus = SheetStatusEnum.Rejected;
        sheet.RejectionNote = rejectionNote;
        sheet.ReviewedAt = DateTime.UtcNow;
        sheet.ReviewedById = managerId;
        sheet.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto((await GetSheetWithRelationsAsync(id))!);
    }

    public async Task<DefectItemResponseDto> AddDefectItemAsync(int sheetId, CreateDefectItemDto dto, int requesterId)
    {
        var sheet = await _db.Sheets.FindAsync(sheetId)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.CreatedById != requesterId)
            throw new UnauthorizedAccessException("Non autorizzato");

        if (sheet.SheetStatus != SheetStatusEnum.Draft && sheet.SheetStatus != SheetStatusEnum.Rejected)
            throw new InvalidOperationException($"Non puoi aggiungere difetti a una scheda in stato '{sheet.SheetStatus}'");

       
        var defect = await _db.DefectCatalogs.FindAsync(dto.DefectCatalogId)
            ?? throw new KeyNotFoundException("Difetto non trovato nel catalogo");

        

        var item = new SheetDefectItem
        {
            SheetId = sheetId,
            DefectCatalogId = dto.DefectCatalogId,
            IsSeen = dto.IsSeen,
            ExtentLow = dto.ExtentLow,
            ExtentMedium = dto.ExtentMedium,
            ExtentHigh = dto.ExtentHigh,
            IntensityLow = dto.IntensityLow,
            IntensityMedium = dto.IntensityMedium,
            IntensityHigh = dto.IntensityHigh,
            IsPs = dto.IsPs,
            IsNa = dto.IsNa,
            IsNr = dto.IsNr,
            IsNp = dto.IsNp,
            Notes = dto.Notes
        };

        _db.SheetDefectItems.Add(item);
        await _db.SaveChangesAsync();

     
        await _db.Entry(item).Reference(i => i.DefectCatalog).LoadAsync();

        return MapDefectItemToDto(item);
    }

    public async Task DeleteDefectItemAsync(int sheetId, int itemId, int requesterId)
    {
        var sheet = await _db.Sheets.FindAsync(sheetId)
            ?? throw new KeyNotFoundException("Scheda non trovata");

        if (sheet.CreatedById != requesterId)
            throw new UnauthorizedAccessException("Non autorizzato");

        if (sheet.SheetStatus != SheetStatusEnum.Draft && sheet.SheetStatus != SheetStatusEnum.Rejected)
            throw new InvalidOperationException($"Non puoi rimuovere difetti da una scheda in stato '{sheet.SheetStatus}'");

        var item = await _db.SheetDefectItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.SheetId == sheetId)
            ?? throw new KeyNotFoundException("Difetto non trovato");

        _db.SheetDefectItems.Remove(item);
        await _db.SaveChangesAsync();
    }



    private async Task<Sheet?> GetSheetWithRelationsAsync(int id)
    {
        return await _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.ReviewedBy)
            .Include(s => s.Technicians).ThenInclude(t => t.User)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public SheetResponseDto MapToDto(Sheet s) => new()
    {
        Id = s.Id,
        Code = s.Code,
        Brand = s.Brand,
        Vehicle = s.Vehicle,
        InspectionDate = s.InspectionDate,
        Status = s.SheetStatus.ToString(),
        RejectionNote = s.RejectionNote,
        SubmittedAt = s.SubmittedAt,
        ReviewedAt = s.ReviewedAt,
        CreatedAt = s.CreatedAt,
        CreatedBy = new UserSummaryDto
        {
            Id = s.CreatedBy.Id,
            Name = s.CreatedBy.Name,
            Email = s.CreatedBy.Email,
            Role = s.CreatedBy.Role
        },
        ReviewedBy = s.ReviewedBy == null ? null : new UserSummaryDto
        {
            Id = s.ReviewedBy.Id,
            Name = s.ReviewedBy.Name,
            Email = s.ReviewedBy.Email,
            Role = s.ReviewedBy.Role
        },
        Technicians = s.Technicians.Select(t => new UserSummaryDto
        {
            Id = t.User.Id,
            Name = t.User.Name,
            Email = t.User.Email,
            Role = t.User.Role
        }).ToList(),
        DefectItems = s.DefectItems.Select(MapDefectItemToDto).ToList()
    };

    private static DefectItemResponseDto MapDefectItemToDto(SheetDefectItem i) => new()
    {
        Id = i.Id,
        DefectCatalogId = i.DefectCatalogId,
        DefectCode = i.DefectCatalog.Code,
        DefectCategory = i.DefectCatalog.Category,
        DefectDescription = i.DefectCatalog.Description,
        IsSeen = i.IsSeen,
        Gravity = i.DefectCatalog.Gravity,
        ExtentLow = i.ExtentLow.GetValueOrDefault(),
        ExtentMedium = i.ExtentMedium.GetValueOrDefault(),
        ExtentHigh = i.ExtentHigh.GetValueOrDefault(),
        IntensityLow = i.IntensityLow.GetValueOrDefault(),
        IntensityMedium = i.IntensityMedium.GetValueOrDefault(),
        IntensityHigh = i.IntensityHigh.GetValueOrDefault(),
        IsPs = i.IsPs.GetValueOrDefault(),
        IsNa = i.IsNa.GetValueOrDefault(),
        IsNr = i.IsNr.GetValueOrDefault(),
        IsNp = i.IsNp.GetValueOrDefault(),
        HasPhoto = i.HasPhoto,
        Notes = i.Notes
    };
}