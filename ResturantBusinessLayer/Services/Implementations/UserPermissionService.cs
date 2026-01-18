using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.UnitOfWork;
using ResturantBusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _uow;

        public UserPermissionService(
            UserManager<User> userManager,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _uow = uow;
        }

        public async Task<IEnumerable<string>> GetUserPermissionCodesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
            {
                return Enumerable.Empty<string>();
            }

            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Any())
            {
                return Enumerable.Empty<string>();
            }

            // Get role IDs
            var roles = await _uow.Roles.Query()
                .Where(r => userRoles.Contains(r.Name!))
                .Select(r => r.Id)
                .ToListAsync();

            if (!roles.Any())
            {
                return Enumerable.Empty<string>();
            }

            // Get permission IDs for these roles
            var permissionIds = await _uow.RolePermissions.Query()
                .Where(rp => roles.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToListAsync();

            if (!permissionIds.Any())
            {
                return Enumerable.Empty<string>();
            }

            // Get permission codes
            var permissions = await _uow.Permissions.Query()
                .Where(p => permissionIds.Contains(p.Id) && !string.IsNullOrEmpty(p.Code))
                .Select(p => p.Code!)
                .ToListAsync();

            return permissions.Distinct();
        }

    }
}