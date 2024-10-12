using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Domain.Entities.ProductEntities;
public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;

    //public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    // -- We Don't Bring Products From Product Category So We Don't Need Navigation Property
    // -- But EF Consider This Relation ONE-ONE And We Need It ONE-MANY So We Modified It In Fluent API
}