using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Services;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/sheets")]
[Authorize] 
public class SheetsController : ControllerBase
{
    private readonly ISheetService _sheetService;

    public SheetsController(ISheetService sheetService)
    {
        _sheetService = sheetService;
    }

  
    private int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException());

    private string GetCurrentUserRole() =>
        User.FindFirst(ClaimTypes.Role)?.Value ?? "";

    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var sheets = role == "manager"
            ? await _sheetService.GetAllAsync()
            : await _sheetService.GetByMechanicAsync(userId);

        return Ok(sheets);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var sheet = await _sheetService.GetByIdAsync(id, userId, role);

        return sheet == null ? NotFound(new { message = "Scheda non trovata" }) : Ok(sheet);
    }


    [HttpPost]
    [Authorize(Roles = "mechanic")]
    public async Task<IActionResult> Create([FromBody] CreateSheetDto dto)
    {
        var userId = GetCurrentUserId();
        var sheet = await _sheetService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = sheet.Id }, sheet);
    }

    
    [HttpPut("{id}")]
    [Authorize(Roles = "mechanic")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSheetDto dto)
    {
        var userId = GetCurrentUserId();
        var sheet = await _sheetService.UpdateAsync(id, dto, userId);
        return Ok(sheet);
    }

   
    [HttpPost("{id}/submit")]
    [Authorize(Roles = "mechanic")]
    public async Task<IActionResult> Submit(int id)
    {
        var userId = GetCurrentUserId();
        var sheet = await _sheetService.SubmitAsync(id, userId);
        return Ok(sheet);
    }

    
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> Approve(int id)
    {
        var managerId = GetCurrentUserId();
        var sheet = await _sheetService.ApproveAsync(id, managerId);
        return Ok(sheet);
    }

   
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectSheetDto dto)
    {
        var managerId = GetCurrentUserId();
        var sheet = await _sheetService.RejectAsync(id, managerId, dto.RejectionNote);
        return Ok(sheet);
    }
}