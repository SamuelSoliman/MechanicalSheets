using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Models;


public class DefectCatalog
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;    
    public DefectCategoryEnum Category { get; set; } = DefectCategoryEnum.Bodywork;   
    public string Description { get; set; } = string.Empty;
    public byte Gravity { get; set; }

    public ICollection<SheetDefectItem> DefectItems { get; set; } = new List<SheetDefectItem>();
}