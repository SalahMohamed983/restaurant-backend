using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResturantBusinessLayer.Dtos.Orders;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;
using ResturantDataAccessLayer.Entities;
using ResturantBusinessLayer.Mappers;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(OrderDto dto)
        {
            var entity = _mapper.Map(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            // Begin transaction
            using var tx = await _uow.BeginTransactionAsync();
            try
            {
                await _uow.Orders.AddAsync(entity);

                // Add order items if any
                if (dto.OrderItems != null && dto.OrderItems.Any())
                {
                    foreach (var itemDto in dto.OrderItems)
                    {
                        var itemEntity = _mapper.Map(itemDto);
                        itemEntity.Id = Guid.NewGuid();
                        itemEntity.OrderId = entity.Id;
                        await _uow.OrderItems.AddAsync(itemEntity);
                    }
                }

                await _uow.SaveChangesAsync();
                await tx.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            // include order items
            var entities = _uow.Orders.Query().Include(o => o.OrderItems);
            var list = await entities.ToListAsync();
            return list.Select(o => _mapper.Map(o));
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var o = await _uow.Orders.Query().Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return null;
            return _mapper.Map(o);
        }
    }
}
