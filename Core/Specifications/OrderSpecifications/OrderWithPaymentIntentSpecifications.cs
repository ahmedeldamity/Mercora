using Core.Entities.OrderEntities;

namespace Core.Specifications.OrderSpecifications;
public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order>
{
    public OrderWithPaymentIntentSpecifications(string paymentIntentId)
    {
        WhereCriteria = O => O.PaymentIntentId == paymentIntentId;
    }
}