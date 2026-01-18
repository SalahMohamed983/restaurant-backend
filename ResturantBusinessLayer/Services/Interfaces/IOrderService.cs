using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Orders;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<Guid> CreateAsync(OrderDto dto);
    }
}
