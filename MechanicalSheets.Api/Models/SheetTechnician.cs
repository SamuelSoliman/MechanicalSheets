namespace MechanicalSheets.Api.Models;


public class SheetTechnician
{
    public int SheetId { get; set; }
    public int UserId { get; set; }

    
    public Sheet Sheet { get; set; } = null!;
    public User User { get; set; } = null!;
}