namespace DotNetCore_ECommerce.Dtos
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageCover { get; set; }
        public string[] Images { get; set; }
        public decimal Quantity { get; set; }
        public decimal RatingsAverage { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public string Brand { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
