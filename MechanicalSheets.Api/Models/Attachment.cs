namespace MechanicalSheets.Api.Models;

public class Attachment
{
    public int Id { get; set; }
    public int SheetDefectId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    
    public SheetDefectItem SheetDefectItem { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}