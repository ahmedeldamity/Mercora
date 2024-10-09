using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Domain.Entities.IdentityEntities;
public class UserAddress: BaseEntity
{
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string Street { get; set; } = null!;
	public string City { get; set; } = null!;
	public string Country { get; set; } = null!;
	public string AppUserId { get; set; } = null!; // FK For User Table | and we don't need to write it in fluent api because his name => 'tableName+Id' AppUserId
}