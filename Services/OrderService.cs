using AutoMapper;
using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderListDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.CreatedAt).ToListAsync();

        return _mapper.Map<IEnumerable<OrderListDto>>(orders);
    }

    public async Task<IEnumerable<OrderListDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OrderListDto>>(orders);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto)
    {
        if (createOrderDto.Items == null || !createOrderDto.Items.Any())
            throw new ArgumentException("Order must contain at least one item.");
        
        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            ShippingAddress = createOrderDto.ShippingAddress,
            Status = "Pending",
            Notes = createOrderDto.Notes,
        };

        decimal totalAmount = 0;

        foreach (var itemDto in createOrderDto.Items)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            
            if (product == null)
            {
                throw new Exception($"Product with ID {itemDto.ProductId} not found");
            }

            if (product.Stock < itemDto.Quantity)
            {
                throw new Exception($"Insufficient stock for product: {product.Name}");
            }

            var subtotal = product.Price * itemDto.Quantity;
            totalAmount += subtotal;

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal
            };

            order.OrderItems.Add(orderItem);

            // Update product stock
            product.Stock -= itemDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        order.TotalAmount = totalAmount;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return await GetOrderByIdAsync(order.Id) ?? throw new Exception("Failed to create order");
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        // _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int id, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null || order.Status == "Cancelled")
            return false;

        if (order.Status != "Pending")
        {
            throw new Exception("Only pending orders can be cancelled");
        }

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;

        // Restock 
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Stock += item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }


    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}