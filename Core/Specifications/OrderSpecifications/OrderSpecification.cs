using Core.Entities.OrderEntities;

namespace Core.Specifications.OrderSpecifications;
public class OrderSpecification : BaseSpecifications<Order>
{
    public OrderSpecification(string buyerEmail) // For Get All Orders For Specific User
    {
        WhereCriteria = P => P.BuyerEmail == buyerEmail;

        IncludesCriteria.Add(P => P.DeliveryMethod);
        IncludesCriteria.Add(P => P.Items);

        OrderByDesc = P => P.OrderDate;
    }

    public OrderSpecification(string buyerEmail, int orderId) // For Get Specific Order For Specific User
    {
        WhereCriteria = P => P.BuyerEmail == buyerEmail && P.Id == orderId;

        IncludesCriteria.Add(P => P.DeliveryMethod);
        IncludesCriteria.Add(P => P.Items);
    }
}