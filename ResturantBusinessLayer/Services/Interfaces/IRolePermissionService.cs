using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Users;
using System;
using System.Collections.Generic;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IRolePermissionService
    {
        Task AssignMultiplePermissionsAsync(AssignMultiplePermissionsDto dto);
        Task RemovePermissionAsync(RolePermissionDto dto);
        Task RemoveAllPermissionsAsync(Guid roleId);
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId);
    }
}
