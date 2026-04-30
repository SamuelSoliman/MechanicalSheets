using System.ComponentModel.DataAnnotations;

namespace MechanicalSheets.Api.DTOs;
public class CreateSheetDto
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Brand { get; set; }
    public string? Vehicle { get; set; }

    [Required]
    public DateOnly InspectionDate { get; set; }

    public List<int> TechnicianIds { get; set; } = new();
}
