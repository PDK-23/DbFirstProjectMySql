
namespace DbFirstProjectMySql.Application.Interfaces
{
    using DbFirstProjectMySql.Application.DTOs;

    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto, int userId);
        Task UpdateAsync(ProductDto dto);
        Task DeleteAsync(int id);
    }

}
