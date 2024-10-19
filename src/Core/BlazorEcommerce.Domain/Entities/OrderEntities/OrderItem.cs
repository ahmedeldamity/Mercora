namespace BlazorEcommerce.Domain.Entities.OrderEntities;
public class OrderItem: BaseEntity
{
    public OrderItem() { /* we create this constructor because EF need it while migration to make instance from this class */ }
    public OrderItem(ProductOrderItem product, decimal price, decimal quantity)
    {
        Product = product;
        Price = price;
        Quantity = quantity;
    }

    public ProductOrderItem Product { get; set; } = null!;
    #region explain
    // this is a navigation property
    // so EF will mapped it to database
    // but we don't need that
    // we need take his properties and mapped it in OrderItem table
    // so will make configration for that :)
    #endregion
    
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}