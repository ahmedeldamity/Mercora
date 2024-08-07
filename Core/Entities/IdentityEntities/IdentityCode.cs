namespace Core.Entities.IdentityEntities
{
    public class IdentityCode
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser User { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
    }
}
