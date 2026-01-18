using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Users;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleWithPermissionsDto?> GetByIdWithPermissionsAsync(Guid id);
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<IEnumerable<RoleWithPermissionsDto>> GetAllWithPermissionsAsync();
        Task<Guid> CreateAsync(RoleDto dto);
        Task UpdateAsync(RoleDto dto);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string name);
    }
}
