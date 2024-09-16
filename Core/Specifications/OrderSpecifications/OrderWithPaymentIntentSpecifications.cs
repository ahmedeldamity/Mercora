using Core.Entities.OrderEntities;

namespace Core.Specifications.OrderSpecifications;
public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order>
{
    public OrderWithPaymentIntentSpecifications(string paymentIntentId)
    {
        WhereCriteria = o => o.PaymentIntentId == paymentIntentId;
        IncludesCriteria.Add(o => o.Items);
    }
}