using BlazorEcommerce.Domain.Entities.OrderEntities;

namespace BlazorEcommerce.Application.Specifications.OrderSpecifications;
public class OrderSpecification : BaseSpecifications<Order>
{
    public OrderSpecification(string? buyerEmail) // For Get All Orders For Specific User
    {
        WhereCriteria = p => p.BuyerEmail == buyerEmail;

        IncludesCriteria.Add(p => p.DeliveryMethod);
        IncludesCriteria.Add(p => p.Items);

        OrderByDesc = p => p.OrderDate;
    }

    public OrderSpecification(string? buyerEmail, int orderId) // For Get Specific Order For Specific User
    {
        WhereCriteria = p => p.BuyerEmail == buyerEmail && p.Id == orderId;

        IncludesCriteria.Add(p => p.DeliveryMethod);
        IncludesCriteria.Add(p => p.Items);
    }
}