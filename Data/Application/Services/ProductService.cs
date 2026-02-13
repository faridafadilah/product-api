using AuthApi.Application.Interfaces;
using AuthApi.Application.Repository.Interfaces;
using AuthApi.Models;
using AuthApi.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMemoryCache _cache;
        private readonly IProductRepository _repository;

        private const string CachePrefix = "products_";

        public ProductService(IProductRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ServiceResult<object>> GetAllAsync(string? search, string? category, int page, int limit)
        {
            page = page <= 0 ? 1 : page;
            limit = limit <= 0 ? 10 : limit;

            string cacheKey = $"{CachePrefix}{search}_{category}_{page}_{limit}";

            // Try get from cache
            if (_cache.TryGetValue(cacheKey, out object? cachedResult))
            {
                return ServiceResult<object>.Ok(cachedResult!);
            }

            var query = await _repository.GetQueryableAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));

            if (!string.IsNullOrWhiteSpace(category))
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

            // Cache configuration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(cacheKey, result, cacheOptions);

            return ServiceResult<object>.Ok(result);
        }

        public async Task<ServiceResult<Product>> GetByIdAsync(int id)
        {
            string cacheKey = $"{CachePrefix}id_{id}";

            if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
            {
                return ServiceResult<Product>.Ok(cachedProduct!);
            }

            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return ServiceResult<Product>.Fail("Product not found.");

            _cache.Set(cacheKey, product, TimeSpan.FromMinutes(1));

            return ServiceResult<Product>.Ok(product);
        }

        public async Task<ServiceResult<Product>> CreateAsync(Product product)
        {
            var created = await _repository.CreateAsync(product);

            ClearProductCache();

            return ServiceResult<Product>.Ok(created, "Product created successfully.");
        }

        public async Task<ServiceResult<Product>> UpdateAsync(Product product)
        {
            var updated = await _repository.UpdateAsync(product);

            ClearProductCache();

            return ServiceResult<Product>.Ok(updated, "Product updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return ServiceResult<bool>.Fail("Product not found.");

            ClearProductCache();

            return ServiceResult<bool>.Ok(true, "Product deleted successfully.");
        }

        // Clear all product cache
        private void ClearProductCache()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}
