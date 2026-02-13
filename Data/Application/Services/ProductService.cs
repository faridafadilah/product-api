using AuthApi.Application.Interfaces;
using AuthApi.Application.Repository.Interfaces;
using AuthApi.Models;
using AuthApi.Models.Common;
namespace AuthApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<Product>>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return ServiceResult<IEnumerable<Product>>.Ok(data);
        }

        public async Task<ServiceResult<Product>> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return ServiceResult<Product>.Fail("Product not found.");

            return ServiceResult<Product>.Ok(product);
        }

        public async Task<ServiceResult<Product>> CreateAsync(Product product)
        {
            var created = await _repository.CreateAsync(product);
            return ServiceResult<Product>.Ok(created, "Product created successfully.");
        }

        public async Task<ServiceResult<Product>> UpdateAsync(Product product)
        {
            var updated = await _repository.UpdateAsync(product);
            return ServiceResult<Product>.Ok(updated, "Product updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return ServiceResult<bool>.Fail("Product not found.");

            return ServiceResult<bool>.Ok(true, "Product deleted successfully.");
        }
    }
}
