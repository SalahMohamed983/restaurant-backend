using ResturantBusinessLayer.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
    
        Task<bool> ExistsAsync(string code);
    }
}
