namespace Core.Entities.Product_Entities;
public class Product : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string ImageCover { get; set; } = null!;

    public string[] Images { get; set; } = [];

    public decimal Quantity { get; set; }

    public decimal RatingsAverage { get; set; }

    //[ForeignKey(nameof(Product.Brand))]  // With this data annotation we can solve problem (But We Don't Named ProductBrandId So EF Don't Know This FK) but we solve it with Fluent API
    public int BrandId { get; set; } // FK - ProductBrand - But We Don't Named ProductBrandId So EF Don't Know This FK So We Make It In Fluent API

    //[InverseProperty(nameof(ProductBrand.Products))] We use this data annotation to link with navigation property in product class (but we use it if exist many navigation property)
    public ProductBrand Brand { get; set; } = new(); // Navigational Property

    //[ForeignKey(nameof(Product.Category))]  // With this data annotation we can solve problem (But We Don't Named ProductCategoryId So EF Don't Know This FK) but we solve it with Fluent API
    public int CategoryId { get; set; } // FK - ProductCategory - But We Don't Named ProductCategoryId So EF Don't Know This FK So We Make It In Fluent API

    //[InverseProperty(nameof(ProductBrand.Products))] We use this data annotation to link with navigation property in product class (but we use it if exist many navigation property)
    public ProductCategory Category { get; set; } = new(); // Navigational Property
}