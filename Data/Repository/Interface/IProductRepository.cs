
using AuthApi.Models;

namespace AuthApi.Application.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<IQueryable<Product>> GetQueryableAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
