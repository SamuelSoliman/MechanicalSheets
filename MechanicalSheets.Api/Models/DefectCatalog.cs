using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Models;


public class DefectCatalog
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;    
    public DefectCategoryEnum Category { get; set; } = DefectCategoryEnum.Bodywork;   
  

    
    public ICollection<SheetDefectItem> DefectItems { get; set; } = new List<SheetDefectItem>();
}