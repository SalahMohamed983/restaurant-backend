using ResturantBusinessLayer.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
        Task<int> CreateAsync(PermissionDto dto);
        Task UpdateAsync(PermissionDto dto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string code);
    }
}
