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
        var sheets = await _db.Sheets
           .Include(s => s.CreatedBy)
           .Include(s => s.DefectItems)
           .ToListAsync();

        var sheetDtos = sheets
            .Select(s => _sheetService.MapToDto(s))
            .ToList();


        return Ok(sheetDtos);
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