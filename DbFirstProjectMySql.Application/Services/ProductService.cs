using AutoMapper;
using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.Interfaces;
using DbFirstProjectMySql.Infrastructure.Entities;
using DbFirstProjectMySql.Infrastructure.IUnitOfWork;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto, int userId)
    {
        var product = _mapper.Map<Product>(dto);
        product.UserId = userId;
        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateAsync(ProductDto dto)
    {
        // Lấy entity đang tracked trong DbContext
        var existing = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
        if (existing == null) return;

        // Map thủ công hoặc dùng AutoMapper để map dto vào entity đã tracked
        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Price = dto.Price;
        // Không cần gọi Update() vì existing đã được tracked

        await _unitOfWork.SaveAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product != null)
        {
            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.SaveAsync();
        }
    }
}
