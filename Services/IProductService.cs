namespace ECommerceApi.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto?> GetProductBySlugAsync(string slug);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateStockAsync(int id, int quantity);
}