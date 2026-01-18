using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Dtos.Orders;
using ResturantBusinessLayer.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            // Customers can only view their own orders
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (userRoles.Contains("Customer") && order.CustomerId != userId)
                {
                    return Forbid();
                }
            }

            return Ok(order);
        }

        [HttpPost]
        //[RequirePermission("ORDER_CREATE")]
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set customer ID from current user if customer
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (userRoles.Contains("Customer"))
                {
                    orderDto.CustomerId = userId;
                }
            }

            try
            {
                var orderId = await _orderService.CreateAsync(orderDto);
                return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId, message = "Order created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

   
   
    }
}
