using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/defect-catalog")]
[Authorize]
public class DefectCatalogController : ControllerBase
{
    private readonly AppDbContext _db;

    public DefectCatalogController(AppDbContext db)
    {
        _db = db;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var catalog = await _db.DefectCatalogs
            .OrderBy(d => d.Category)
            .ThenBy(d => d.Code)
            .Select(d => new
            {
                d.Id,
                d.Code,
                d.Category,
                d.Description,
                d.Gravity
            })
            .ToListAsync();

        return Ok(catalog);
    }
}