
using System.ComponentModel.DataAnnotations;

namespace MechanicalSheets.Api.DTOs;

public class CreateDefectItemDto
{
    [Required]
    public int DefectCatalogId { get; set; }

    [Required]
    public bool IsSeen { get; set; }

    public bool? ExtentLow { get; set; }
    public bool? ExtentMedium { get; set; }
    public bool? ExtentHigh { get; set; }

    public bool? IntensityLow { get; set; }
    public bool? IntensityMedium { get; set; }
    public bool? IntensityHigh { get; set; }

    public bool? IsPs { get; set; }
    public bool? IsNa { get; set; }
    public bool? IsNr { get; set; }
    public bool? IsNp { get; set; }
    public bool HasPhoto { get; set; }
    public string? Notes { get; set; }
}