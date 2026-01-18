using ResturantBusinessLayer.Dtos.Common;
using ResturantBusinessLayer.Dtos.Users;
using System;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponseDto<UserDto>> GetAllUsersAsync(PagedRequestDto request);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
