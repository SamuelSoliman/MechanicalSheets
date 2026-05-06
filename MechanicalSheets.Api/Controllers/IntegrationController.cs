using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;
using MechanicalSheets.Api.Services;
using MechanicalSheets.Api.Enums;
using MechanicalSheets.Api.DTOs;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/integration")]
public class IntegrationController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISheetService _sheetService;

    public IntegrationController(AppDbContext db, ISheetService sheetService)
    {
        _db = db;
        _sheetService = sheetService;
    }

    
    [HttpGet("sheets")]
    public async Task<IActionResult> GetSheets([FromQuery] SheetStatusEnum? status = null)
    {
        var query = _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.SheetStatus == status.Value);

        var sheets = await query
            .Select(s => new
            {
                s.Id,
                s.Code,
                s.Brand,
                s.Vehicle,
                s.SheetStatus,
                s.InspectionDate,
                s.SubmittedAt,
                s.ReviewedAt,
                CreatedBy = s.CreatedBy.Name,
                DefectCount = s.DefectItems.Count
            })
            .ToListAsync();

        

        return Ok(sheets);
    }

    
    [HttpGet("sheets/{id}")]
    public async Task<IActionResult> GetSheet(int id)
    {
        var sheet = await _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.ReviewedBy)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sheet == null) return NotFound(new { message = "Scheda non trovata" });
       var sheetDto = _sheetService.MapToDto(sheet);
        return Ok(sheetDto);
    }

    [HttpPut("sheets/{id}/status")]
    public async Task<IActionResult> ChangeSheetStatus(int id, [FromBody] ChangeSheetStatusDto dto)
    {
        var sheet = await _db.Sheets.FindAsync(id);
        if (sheet == null)
            return NotFound(new { message = "Scheda non trovata" });

        // Block any transitions out of Closed state (terminal state)
        if (sheet.SheetStatus == SheetStatusEnum.Closed)
            return BadRequest(new { message = "Una scheda chiusa non può essere modificata" });

        // Rejected sheets can only return to Draft
        if (sheet.SheetStatus == SheetStatusEnum.Rejected && dto.NewStatus != SheetStatusEnum.Draft && dto.NewStatus != SheetStatusEnum.Closed)
            return BadRequest(new { message = "Una scheda rifiutata può solo tornare in bozza o essere chiusa" });

        // Draft sheets cannot transition directly to Closed
        if (sheet.SheetStatus == SheetStatusEnum.Draft && dto.NewStatus == SheetStatusEnum.Closed)
            return BadRequest(new { message = "Una scheda in bozza non può essere chiusa direttamente" });

        // Rejection note is required when rejecting
        if (dto.NewStatus == SheetStatusEnum.Rejected && string.IsNullOrWhiteSpace(dto.RejectionNote))
            return BadRequest(new { message = "Nota di rifiuto obbligatoria quando si rifiuta una scheda" });

        sheet.SheetStatus = dto.NewStatus;
        sheet.UpdatedAt = DateTime.UtcNow;

        if (dto.NewStatus == SheetStatusEnum.Submitted)
        {
            sheet.SubmittedAt = DateTime.UtcNow;
        }
        else if (dto.NewStatus == SheetStatusEnum.Approved)
        {
            sheet.ReviewedAt = DateTime.UtcNow;
        }
        else if (dto.NewStatus == SheetStatusEnum.Rejected)
        {
            sheet.RejectionNote = dto.RejectionNote;
            sheet.ReviewedAt = DateTime.UtcNow;
        }
        else if (dto.NewStatus == SheetStatusEnum.Closed)
        {
            sheet.ReviewedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return Ok(new { message = "Stato scheda aggiornato con successo" });
    }

}