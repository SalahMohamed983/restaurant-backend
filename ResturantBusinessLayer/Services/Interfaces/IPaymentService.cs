using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Payments;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<Guid> CreateAsync(PaymentDto dto);
    }
}
