

namespace MechanicalSheets.Api.DTOs;
public class UpdateSheetDto
{
    public string? Code { get; set; }
    public string? Brand { get; set; }
    public string? Vehicle { get; set; }
    public DateOnly? InspectionDate { get; set; }
    public List<int>? TechnicianIds { get; set; }
}