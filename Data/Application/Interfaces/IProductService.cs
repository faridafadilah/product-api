using AuthApi.Models;
using AuthApi.Models.Common;

namespace AuthApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<object>> GetAllAsync(
        string? search,
        string? category,
        int page,
        int limit);
        Task<ServiceResult<Product>> GetByIdAsync(int id);
        Task<ServiceResult<Product>> CreateAsync(Product product);
        Task<ServiceResult<Product>> UpdateAsync(Product product);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }

}
