using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;
using ResturantDataAccessLayer.Entities;
using ResturantBusinessLayer.Mappers;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public RolePermissionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        

        public async Task AssignMultiplePermissionsAsync(AssignMultiplePermissionsDto dto)
        {
            // Validate role exists
            var role = await _uow.Roles.GetByIdAsync(dto.RoleId);
            if (role == null)
                throw new InvalidOperationException($"Role with ID {dto.RoleId} not found.");

            // Validate all permissions exist
            var allPermissions = await _uow.Permissions.GetAllAsync();
            var invalidPermissions = dto.PermissionIds
                .Where(id => !allPermissions.Any(p => p.Id == id))
                .ToList();

            if (invalidPermissions.Any())
            {
                throw new InvalidOperationException($"Invalid permission IDs: {string.Join(", ", invalidPermissions)}");
            }

            // Get existing permissions for this role
            var existingPermissions = await _uow.RolePermissions.Query()
                .Where(rp => rp.RoleId == dto.RoleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // Add only new permissions
            var newPermissions = dto.PermissionIds
                .Where(id => !existingPermissions.Contains(id))
                .ToList();

            foreach (var permissionId in newPermissions)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = permissionId
                };
                await _uow.RolePermissions.AddAsync(rolePermission);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task RemovePermissionAsync(RolePermissionDto dto)
        {
            var rolePermission = await _uow.RolePermissions.Query()
                .FirstOrDefaultAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId);

            if (rolePermission == null)
                throw new InvalidOperationException("Permission is not assigned to this role.");

            _uow.RolePermissions.Remove(rolePermission);
            await _uow.SaveChangesAsync();
        }

        public async Task RemoveAllPermissionsAsync(Guid roleId)
        {
            // Validate role exists
            var role = await _uow.Roles.GetByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException($"Role with ID {roleId} not found.");

            var rolePermissions = await _uow.RolePermissions.Query()
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            foreach (var rolePermission in rolePermissions)
            {
                _uow.RolePermissions.Remove(rolePermission);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId)
        {
            var permissionIds = await _uow.RolePermissions.Query()
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var allPermissions = await _uow.Permissions.GetAllAsync();
            var permissions = allPermissions
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => _mapper.Map(p))
                .ToList();

            return permissions;
        }

    }
}
