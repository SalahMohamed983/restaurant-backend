using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Reservations;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ReservationDto>> GetAllAsync();
        Task<Guid> CreateAsync(ReservationDto dto);
        Task ApproveReservationAsync(Guid reservationId, Guid approverUserId);
        Task RejectReservationAsync(Guid reservationId, string reason);
    }
}
