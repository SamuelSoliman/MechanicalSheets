public class ApiKey
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;   
    public string KeyHash { get; set; } = string.Empty; 
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}