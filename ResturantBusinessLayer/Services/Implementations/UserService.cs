using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResturantBusinessLayer.Dtos.Common;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Mappers;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.Common;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserService> _logger;
        private readonly EntityMappers _mapper;

        public UserService(
            UserManager<User> userManager,
            IUnitOfWork uow,
            ILogger<UserService> logger,
            EntityMappers mapper)
        {
            _userManager = userManager;
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResponseDto<UserDto>> GetAllUsersAsync(PagedRequestDto request)
        {
            // Build query with filter
            var query = _uow.Users.Query()
                .Where(u => !u.IsDeleted);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                query = query.Where(u => 
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.FullName != null && u.FullName.Contains(searchTerm)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<User>
            {
                Data = users,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            // Map to DTOs using Mapperly
            var userDtos = new List<UserDto>();

            foreach (var user in pagedResult.Data)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }

            // Convert PagedResult to PagedResponseDto
            return new PagedResponseDto<UserDto>
            {
                Data = userDtos,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map(user);
            userDto.Roles = roles.ToList();
            
            return userDto;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Attempt to delete non-existent or already deleted user: {UserId}", userId);
                return false;
            }

            // Soft delete - mark as deleted instead of hard delete
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Revoke all refresh tokens for this user
                var userTokens = await _uow.RefreshTokens.Query()
                    .Where(rt => rt.UserId == userId && rt.RevokedOn == null)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                    token.RevokedByIp = "User Deleted";
                }

                if (userTokens.Any())
                {
                    _uow.RefreshTokens.UpdateRange(userTokens);
                    await _uow.SaveChangesAsync();
                }

                _logger.LogInformation("User deleted successfully: {UserId}, Email: {Email}", userId, user.Email);
                return true;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to delete user {UserId}. Errors: {Errors}", userId, errors);
            return false;
        }
    }
}
