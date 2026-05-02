using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;
using MechanicalSheets.Api.Services;

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
    public async Task<IActionResult> GetSheets([FromQuery] string? status = null)
    {
        var query = _db.Sheets
            .Include(s => s.CreatedBy)
            .Include(s => s.DefectItems).ThenInclude(i => i.DefectCatalog)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.SheetStatus.ToString() == status);

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

}