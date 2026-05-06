using System.ComponentModel.DataAnnotations;
namespace MechanicalSheets.Api.Enums;
public enum SheetStatusEnum
{
    // 
   [Display(Name = "Bozza")]
    Draft = 0,

    [Display(Name = "Inviato")]
    Submitted = 1,

    [Display(Name = "Approvato")]
    Approved = 2,

    [Display(Name = "Rifiutato")]
    Rejected = 3,

    [Display(Name = "Chiuso")]
    Closed = 4
}