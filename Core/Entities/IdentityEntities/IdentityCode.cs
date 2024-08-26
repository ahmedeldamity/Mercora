namespace Core.Entities.IdentityEntities;
public class IdentityCode
{
    public int Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool ForRegisterationConfirmed { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    public DateTime? ActivationTime { get; set; }
}