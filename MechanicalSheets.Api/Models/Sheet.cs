using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Models;

public class Sheet
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Vehicle { get; set; }
    public DateOnly InspectionDate { get; set; }
    public SheetStatusEnum SheetStatus { get; set; } = SheetStatusEnum.Draft;
    public string? RejectionNote { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int CreatedById { get; set; }
    public int? ReviewedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    
    public User CreatedBy { get; set; } = null!;
    public User? ReviewedBy { get; set; }
    public ICollection<SheetTechnician> Technicians { get; set; } = new List<SheetTechnician>();
    public ICollection<SheetDefectItem> DefectItems { get; set; } = new List<SheetDefectItem>();
}