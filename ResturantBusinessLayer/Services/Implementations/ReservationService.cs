using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Reservations;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;
using ResturantDataAccessLayer.Entities;
using ResturantBusinessLayer.Mappers;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public ReservationService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(ReservationDto dto)
        {
            var entity = _mapper.Map(dto);
            entity.Id = Guid.NewGuid();
            await _uow.Reservations.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity.Id;
        }

        
        public async Task<IEnumerable<ReservationDto>> GetAllAsync()
        {
            var entities = await _uow.Reservations.GetAllAsync();
            return entities.Select(r => _mapper.Map(r));
        }

        public async Task<ReservationDto?> GetByIdAsync(Guid id)
        {
            var r = await _uow.Reservations.GetByIdAsync(id);
            if (r == null) return null;
            return _mapper.Map(r);
        }

        

        public async Task ApproveReservationAsync(Guid reservationId, Guid approverUserId)
        {
            var e = await _uow.Reservations.GetByIdAsync(reservationId);
            if (e == null) return;
            var oldStatus = e.Status;
            e.Status = ReservationStatus.Approved;
            e.ApprovedByUserId = approverUserId;
            e.ApprovedAt = DateTime.UtcNow;
            _uow.Reservations.Update(e);
            await _uow.ReservationStatusHistories.AddAsync(new ReservationStatusHistory
            {
                Id = Guid.NewGuid(),
                ReservationId = e.Id,
                OldStatus = oldStatus,
                NewStatus = ReservationStatus.Approved,
                ChangedByUserId = approverUserId,
                ChangedAt = DateTime.UtcNow
            });
            await _uow.SaveChangesAsync();
        }

        public async Task RejectReservationAsync(Guid reservationId, string reason)
        {
            var e = await _uow.Reservations.GetByIdAsync(reservationId);
            if (e == null) return;
            var oldStatus = e.Status;
            e.Status = ReservationStatus.Rejected;
            e.Notes = reason;
            _uow.Reservations.Update(e);
            await _uow.ReservationStatusHistories.AddAsync(new ReservationStatusHistory
            {
                Id = Guid.NewGuid(),
                ReservationId = e.Id,
                OldStatus = oldStatus,
                NewStatus = ReservationStatus.Rejected,
                ChangedByUserId = null,
                ChangedAt = DateTime.UtcNow,
                Comment = reason
            });
            await _uow.SaveChangesAsync();
        }
    }
}
