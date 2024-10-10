using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Domain.Entities.IdentityEntities;
public class IdentityCode: BaseEntity
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool ForRegistrationConfirmed { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    public DateTime? ActivationTime { get; set; }
}