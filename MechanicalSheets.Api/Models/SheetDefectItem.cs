namespace MechanicalSheets.Api.Models;

public class SheetDefectItem
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public int DefectCatalogId { get; set; }  // FK intera — più semplice e consistente
    public bool IsSeen { get; set; }
    public byte Gravity { get; set; }

    // Estensione (radio logic — max uno true per gruppo)
    public bool ExtentLow { get; set; }
    public bool ExtentMedium { get; set; }
    public bool ExtentHigh { get; set; }

    // Intensità (radio logic)
    public bool IntensityLow { get; set; }
    public bool IntensityMedium { get; set; }
    public bool IntensityHigh { get; set; }

    public bool IsPs { get; set; }
    public bool IsNa { get; set; }
    public bool IsNr { get; set; }
    public bool IsNp { get; set; }
    public bool HasPhoto { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    
    public InterventionSheet Sheet { get; set; } = null!;
    public DefectCatalog DefectCatalog { get; set; } = null!;
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}