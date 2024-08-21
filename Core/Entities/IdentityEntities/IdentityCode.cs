namespace Core.Entities.IdentityEntities;
public class IdentityCode
{
    public int Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public AppUser User { get; set; } = new();
    public string Code { get; set; } = null!;
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public DateTime? ActivationTime { get; set; }
}