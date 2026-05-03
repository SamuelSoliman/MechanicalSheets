using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.DTOs;

public class ChangeSheetStatusDto
{
    public SheetStatusEnum NewStatus { get; set; }
    public string? RejectionNote { get; set; } // Required when rejecting
}