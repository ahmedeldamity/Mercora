namespace Core.Entities.OrderEntities;
public class Order: BaseEntity
{
    public Order() { /* we create this constractor because EF need it while migration to make instance from this class */ }
    public Order(string buyerEmail, OrderAddress shippingAddress, OrderDeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
    {
        BuyerEmail = buyerEmail;
        ShippingAddress = shippingAddress;
        DeliveryMethod = deliveryMethod;
        Items = items;
        SubTotal = subTotal;
    }

    public string BuyerEmail { get; set; } = null!;

    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.pending;
    #region Explaination
    // In this property
    // we need store in database string not number
    // so we create configuration for it
    #endregion

    public OrderAddress ShippingAddress { get; set; } = new();
    #region Explaination
    // this is a navigation property
    // so EF will mapped it to database
    // but we don't need that
    // we need take his properties and mapped it in Order table
    // so will make configration for that :)
    #endregion

    public OrderDeliveryMethod DeliveryMethod { get; set; } = new();
    #region Explaination
    // this is navagation property one to many
    // so EF will take PK of many as FK in one
    // so EF will take PK of table DeliveryMethod as FK in this table
    #endregion

    public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
    #region Explaination
    // this is navagation property one to many
    // so EF will take PK of many as FK in one
    // so EF will take PK of table Order as FK in OrderItem table
    // -- HashSet to be list unique
    #endregion

    public decimal SubTotal { get; set; } // all salaries of items
    
    public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
    #region Explaination
    // -- here we put Total property but we need that: this property not mapped in database
    // -- because this is derived attribute, because we can bring his value from another attributes (Subtotal + delivery method cost)
    // -- so to make derived attribute we have two ways
    // -- first: with read only property and data annotation [NotMapped]
    // ----- public decimal Total => Subtotal + DeliveryMethod.Cost;
    // -- second: with function
    #endregion

    public string PaymentIntentId { get; set; } = ""; // removed
}