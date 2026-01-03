using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerceApi.DTOs;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrders()
    {
        var orders = IsAdmin()
            ? await _orderService.GetAllOrdersAsync()
            : await _orderService.GetUserOrdersAsync(GetCurrentUserId());

        return Ok(orders);
    }

    // GET: api/orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        // Check if user owns the order or is admin
        if (!IsAdmin() && order.User.Id != GetCurrentUserId())
        {
            return Forbid();
        }

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        if (createOrderDto.Items == null || !createOrderDto.Items.Any())
        {
            return BadRequest(new { message = "Order must contain at least one item" });
        }

        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CreateOrderAsync(userId, createOrderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/orders/5/status
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto updateStatusDto)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto.Status);
        
        if (!success)
        {
            return NotFound(new { message = "Order not found" });
        }

        return NoContent();
    }

    // POST: api/orders/5/cancel
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _orderService.CancelOrderAsync(id, userId);
            
            if (!success)
            {
                return NotFound(new { message = "Order not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}