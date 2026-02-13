using AuthApi.Application.Interfaces;
using AuthApi.Application.Repository.Interfaces;
using AuthApi.Models;
using AuthApi.Models.Common;
using Microsoft.EntityFrameworkCore;
namespace AuthApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<object>> GetAllAsync(
            string? search,
            string? category,
            int page,
            int limit)
        {
            var query = await _repository.GetQueryableAsync();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower() == category.ToLower());

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var result = new
            {
                totalItems,
                page,
                limit,
                totalPages = (int)Math.Ceiling((double)totalItems / limit),
                items
            };

            return ServiceResult<object>.Ok(result);
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
