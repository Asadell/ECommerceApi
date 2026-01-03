using ECommerceApi.DTOs;

namespace ECommerceApi.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderListDto>> GetAllOrdersAsync();
    Task<IEnumerable<OrderListDto>> GetUserOrdersAsync(int userId);
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto);
    Task<bool> UpdateOrderStatusAsync(int id, string status);
    Task<bool> CancelOrderAsync(int id, int userId);
}
