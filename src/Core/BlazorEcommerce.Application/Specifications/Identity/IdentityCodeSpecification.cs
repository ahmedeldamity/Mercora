using BlazorEcommerce.Domain.Entities.IdentityEntities;

namespace BlazorEcommerce.Application.Specifications.Identity;
public class IdentityCodeSpecification : BaseSpecifications<IdentityCode>
{
	public IdentityCodeSpecification(string userId, bool forRegistrationConfirmed = true, bool isActive = false)
	{
		WhereCriteria = p => p.AppUserId == userId && p.ForRegistrationConfirmed == forRegistrationConfirmed;

		OrderBy = p => p.CreationTime;
	}
}