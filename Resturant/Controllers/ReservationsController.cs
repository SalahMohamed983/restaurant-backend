using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Dtos.Reservations;
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
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = $"Reservation with ID {id} not found." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (userRoles.Contains("Customer") && reservation.CustomerId != userId)
                {
                    return Forbid();
                }
            }

            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationDto reservationDto)
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
                    reservationDto.CustomerId = userId;
                }
            }

            try
            {
                var reservationId = await _reservationService.CreateAsync(reservationDto);
                return CreatedAtAction(nameof(GetById), new { id = reservationId }, new { id = reservationId, message = "Reservation created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var approverUserId))
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            try
            {
                await _reservationService.ApproveReservationAsync(id, approverUserId);
                return Ok(new { message = "Reservation approved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _reservationService.RejectReservationAsync(id, dto.Reason);
                return Ok(new { message = "Reservation rejected successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class RejectReservationDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
