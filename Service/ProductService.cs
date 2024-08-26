using Core.Entities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;

namespace Service
{
    public class ProductService(IUnitOfWork _unitOfWork) : IProductService
    {
        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecificationParameters specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);
            var products = await _unitOfWork.Repository<Product>().GetAllAsync(spec);
            return products;
        }
        public async Task<int> GetProductCount(ProductSpecificationParameters specParams)
        {
            var spec = new ProductCountSpecification(specParams);
            var productsCount = await _unitOfWork.Repository<Product>().GetCountAsync(spec);
            return productsCount;
        }
        public async Task<Product?> GetProductAsync(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityAsync(spec);
            return product;
        }
    }
}