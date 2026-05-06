using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MechanicalSheets.Api.Data;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Models;
using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/sheets/{sheetId}/defects/{itemId}/attachments")]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AttachmentsController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException());

    
 
    [HttpGet]
    public async Task<IActionResult> GetAttachments(int sheetId, int itemId)
    {
        var userId = GetCurrentUserId();
        var role   = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

    
        var sheet = await _db.Sheets
            .Include(s => s.Technicians)
            .FirstOrDefaultAsync(s => s.Id == sheetId);

        if (sheet == null)
            return NotFound(new { message = "Scheda non trovata" });

      
        if (role == "mechanic" &&
            sheet.CreatedById != userId &&
            !sheet.Technicians.Any(t => t.UserId == userId))
            return StatusCode(403, new { message = "Non autorizzato" });

        var item = await _db.SheetDefectItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.SheetId == sheetId);

        if (item == null)
            return NotFound(new { message = "Difetto non trovato" });

        var attachments = await _db.Attachments
            .Include(a => a.UploadedBy)
            .Where(a => a.SheetDefectId == itemId)
            .Select(a => new AttachmentResponseDto
            {
                Id = a.Id,
                FileName = a.FileName,
                MimeType = a.MimeType,
                FileSize = a.FileSize,
                UploadedAt = a.UploadedAt,
                UploadedBy = a.UploadedBy.Name,
                Url = $"/api/files/{a.FilePath}"
            })
            .ToListAsync();

        return Ok(attachments);
    }


   [HttpPost]
   [Authorize(Roles = "mechanic")]
    public async Task<IActionResult> Upload(int sheetId, int itemId, IFormFile file)
    {
       
        var sheet = await _db.Sheets
            .Include(s => s.Technicians)
            .FirstOrDefaultAsync(s => s.Id == sheetId);
        if (sheet == null)
            return NotFound(new { message = "Scheda non trovata" });

        var userId = GetCurrentUserId();
        if (sheet.CreatedById != userId && !sheet.Technicians.Any(t => t.UserId == userId))
            return StatusCode(403, new { message = "Non autorizzato" });

        
        if (sheet.SheetStatus == SheetStatusEnum.Approved || sheet.SheetStatus == SheetStatusEnum.Submitted || sheet.SheetStatus == SheetStatusEnum.Closed)
            return BadRequest(new { message = $"Non puoi aggiungere foto a una scheda in stato '{sheet.SheetStatus}'" });

      
        var item = await _db.SheetDefectItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.SheetId == sheetId);

        if (item == null)
            return NotFound(new { message = "Difetto non trovato" });

       
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Solo immagini JPEG, PNG, WEBP sono accettate" });

        
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File troppo grande (max 5MB)" });

       
        var uploadPath = _config["Storage:UploadPath"] ?? "/tmp/mechanical_sheets_uploads";
        Directory.CreateDirectory(uploadPath);

       
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(uploadPath, fileName);

        
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

      
        var attachment = new Attachment
        {
            SheetDefectId = itemId,
            FileName = file.FileName,       
            FilePath = fileName,             
            MimeType = file.ContentType,
            FileSize = file.Length,
            UploadedById = userId
        };

        _db.Attachments.Add(attachment);

      
        item.HasPhoto = true;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAttachments), new { sheetId, itemId }, new AttachmentResponseDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            MimeType = attachment.MimeType,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.UploadedAt,
            UploadedBy = _db.Users.Where(u => u.Id == userId).Select(u => u.Name).FirstOrDefault() ?? "Unknown",
            Url = $"/api/files/{attachment.FilePath}"
        });
    }

  
    [HttpDelete("{attachmentId}")]
    [Authorize(Roles = "mechanic")]
    public async Task<IActionResult> Delete(int sheetId, int itemId, int attachmentId)
    {
        var sheet = await _db.Sheets
            .Include(s => s.Technicians)
            .FirstOrDefaultAsync(s => s.Id == sheetId);
        if (sheet == null) return NotFound(new { message = "Scheda non trovata" });

        var userId = GetCurrentUserId();
        if (sheet.CreatedById != userId && !sheet.Technicians.Any(t => t.UserId == userId))
            return StatusCode(403, new { message = "Non autorizzato" });

        if (sheet.SheetStatus == SheetStatusEnum.Approved || sheet.SheetStatus == SheetStatusEnum.Submitted)
            return BadRequest(new { message = "Non puoi rimuovere foto da una scheda approvata" });

        var attachment = await _db.Attachments
            .FirstOrDefaultAsync(a => a.Id == attachmentId && a.SheetDefectId == itemId);

        if (attachment == null)
            return NotFound(new { message = "Allegato non trovato" });

        
        var uploadPath = _config["Storage:UploadPath"] ?? "/tmp/mechanical_sheets_uploads";
        var fullPath = Path.Combine(uploadPath, attachment.FilePath);
        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);

        _db.Attachments.Remove(attachment);

       
        var remainingPhotos = await _db.Attachments
            .CountAsync(a => a.SheetDefectId == itemId && a.Id != attachmentId);

        if (remainingPhotos == 0)
        {
            var item = await _db.SheetDefectItems.FindAsync(itemId);
            if (item != null)
            {
                item.HasPhoto = false;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}