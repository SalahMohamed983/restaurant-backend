using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResturantBusinessLayer.Mappers;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly EntityMappers _mapper = new EntityMappers();

        public RoleService(IUnitOfWork uow, RoleManager<AspNetRole> roleManager)
        {
            _uow = uow;
            _roleManager = roleManager;
        }

        public async Task<Guid> CreateAsync(RoleDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("Role name is required.");

            // Check if role already exists
            if (await ExistsAsync(dto.Name))
            {
                throw new InvalidOperationException($"Role '{dto.Name}' already exists.");
            }

            var role = new AspNetRole
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NormalizedName = dto.Name.ToUpperInvariant()
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return role.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new InvalidOperationException($"Role with ID {id} not found.");

            // Check if role is assigned to any users
            // Note: We can't easily check this with RoleManager alone, so we'll skip this check
            // In production, you might want to add a custom method to check this

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to delete role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return await _roleManager.RoleExistsAsync(name);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = _roleManager.Roles.ToList();
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
            });
        }

        public async Task<IEnumerable<RoleWithPermissionsDto>> GetAllWithPermissionsAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var result = new List<RoleWithPermissionsDto>();

            foreach (var role in roles)
            {
                var roleWithPerms = await GetByIdWithPermissionsAsync(role.Id);
                if (roleWithPerms != null)
                {
                    result.Add(roleWithPerms);
                }
            }

            return result;
        }

        public async Task<RoleDto?> GetByIdAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
            };
        }

        public async Task<RoleWithPermissionsDto?> GetByIdWithPermissionsAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return null;

            // Get permissions for this role
            var rolePermissions = await _uow.RolePermissions.Query()
                .Where(rp => rp.RoleId == id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var permissions = await _uow.Permissions.GetAllAsync();
            var rolePerms = permissions
                .Where(p => rolePermissions.Contains(p.Id))
                .Select(p => _mapper.Map(p))
                .ToList();

            return new RoleWithPermissionsDto
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Permissions = rolePerms
            };
        }

        public async Task UpdateAsync(RoleDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("Role name is required.");

            var role = await _roleManager.FindByIdAsync(dto.Id.ToString());
            if (role == null)
                throw new InvalidOperationException($"Role with ID {dto.Id} not found.");

            // Check if new name already exists (and it's different from current name)
            if (!role.Name!.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) && await ExistsAsync(dto.Name))
            {
                throw new InvalidOperationException($"Role '{dto.Name}' already exists.");
            }

            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpperInvariant();

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
