using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    /// <summary>
    /// Service to get user permissions from their roles
    /// </summary>
    public interface IUserPermissionService
    {
        /// <summary>
        /// Get all permission codes for a user based on their roles
        /// </summary>
        Task<IEnumerable<string>> GetUserPermissionCodesAsync(Guid userId);


    }
}