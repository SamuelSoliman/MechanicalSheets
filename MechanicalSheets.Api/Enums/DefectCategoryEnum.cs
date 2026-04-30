using System.ComponentModel.DataAnnotations;
namespace MechanicalSheets.Api.Enums;
public enum DefectCategoryEnum
{
    [Display(Name = "Carrozzeria")]
    Bodywork = 0,

    [Display(Name = "Illuminazione")]
    Lighting = 1,

    [Display(Name = "Fissaggi")]
    Fasteners = 2,

    [Display(Name = "Telaio")]
    Frame = 3
}