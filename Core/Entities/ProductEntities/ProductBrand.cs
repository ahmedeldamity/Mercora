namespace Core.Entities;
public class ProductBrand : BaseEntity
{
    public string Name { get; set; } = null!;
    public string ImageCover { get; set; } = null!;

    //public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    // -- We Don't Bring Products From Product Brand So We Don't Need Navigation Property
    // -- But EF Consider This Relation ONE-ONE And We Need It ONE-MANY So We Modified It In Fluent API
}