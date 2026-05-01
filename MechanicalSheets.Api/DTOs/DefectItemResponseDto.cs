using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.DTOs;
public class DefectItemResponseDto
{
    public int Id { get; set; }
    public int DefectCatalogId { get; set; }

    public string DefectCode { get; set; } = string.Empty;
    public DefectCategoryEnum DefectCategory { get; set; } = DefectCategoryEnum.Bodywork;
    public string DefectDescription { get; set; } = string.Empty;

    public bool IsSeen { get; set; }
    public byte Gravity { get; set; }
    public bool ExtentLow { get; set; }
    public bool ExtentMedium { get; set; }
    public bool ExtentHigh { get; set; }
    public bool IntensityLow { get; set; }
    public bool IntensityMedium { get; set; }
    public bool IntensityHigh { get; set; }
    public bool IsPs { get; set; }
    public bool IsNa { get; set; }
    public bool IsNr { get; set; }
    public bool IsNp { get; set; }
    public bool HasPhoto { get; set; }
    public string? Notes { get; set; }
}
