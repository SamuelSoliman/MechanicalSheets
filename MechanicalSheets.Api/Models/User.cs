namespace MechanicalSheets.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "mechanic"; // "mechanic" | "manager"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    
    public ICollection<Sheet> CreatedSheets { get; set; } = new List<Sheet>();
    public ICollection<Sheet> ReviewedSheets { get; set; } = new List<Sheet>();
    public ICollection<SheetTechnician> SheetTechnicians { get; set; } = new List<SheetTechnician>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}