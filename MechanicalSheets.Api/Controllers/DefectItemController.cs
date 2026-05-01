using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Services;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/sheets/{sheetId}/defects")]
[Authorize(Roles = "mechanic")]
public class DefectItemsController : ControllerBase
{
    private readonly ISheetService _sheetService;

    public DefectItemsController(ISheetService sheetService)
    {
        _sheetService = sheetService;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException());

    
    [HttpPost]
    public async Task<IActionResult> AddDefect(int sheetId, [FromBody] CreateDefectItemDto dto)
    {
        var userId = GetCurrentUserId();
        var item = await _sheetService.AddDefectItemAsync(sheetId, dto, userId);
        return CreatedAtAction(nameof(AddDefect), new { sheetId, id = item.Id }, item);
    }

    
    [HttpDelete("{itemId}")]
    public async Task<IActionResult> DeleteDefect(int sheetId, int itemId)
    {
        var userId = GetCurrentUserId();
        await _sheetService.DeleteDefectItemAsync(sheetId, itemId, userId);
        return NoContent();
    }
}