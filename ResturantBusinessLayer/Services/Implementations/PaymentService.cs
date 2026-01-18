using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Payments;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;
using ResturantDataAccessLayer.Entities;
using ResturantBusinessLayer.Mappers;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public PaymentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(PaymentDto dto)
        {
            var entity = _mapper.Map(dto);
            entity.Id = Guid.NewGuid();
            await _uow.Payments.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var entities = await _uow.Payments.GetAllAsync();
            return entities.Select(p => _mapper.Map(p));
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id)
        {
            var p = await _uow.Payments.GetByIdAsync(id);
            if (p == null) return null;
            return _mapper.Map(p);
        }

    }
}
