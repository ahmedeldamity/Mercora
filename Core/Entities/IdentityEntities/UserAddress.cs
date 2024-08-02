namespace Core.Entities.IdentityEntities
{
	public class UserAddress: BaseEntity
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string AppUserId { get; set; } // FK For User Table | and we don't need to write it in fluent api because his name => 'tableName+Id' AppUserId
	}
}
