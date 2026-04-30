using MechanicalSheets.Api.DTOs;

public class SheetResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Vehicle { get; set; }
    public DateOnly InspectionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionNote { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserSummaryDto CreatedBy { get; set; } = null!;
    public UserSummaryDto? ReviewedBy { get; set; }
    public List<UserSummaryDto> Technicians { get; set; } = new();
    public List<DefectItemResponseDto> DefectItems { get; set; } = new();
}
