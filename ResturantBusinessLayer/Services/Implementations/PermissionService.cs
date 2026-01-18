using Microsoft.EntityFrameworkCore;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Mappers;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public PermissionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> CreateAsync(PermissionDto dto)
        {
            // Check if code already exists
            if (!string.IsNullOrEmpty(dto.Code) && await ExistsAsync(dto.Code))
            {
                throw new InvalidOperationException($"Permission with code '{dto.Code}' already exists.");
            }

            var entity = _mapper.Map(dto);
            await _uow.Permissions.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.Permissions.Query().FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return;

            // Check if permission is assigned to any role
            var isAssigned = await _uow.RolePermissions.Query()
                .AnyAsync(rp => rp.PermissionId == id);

            if (isAssigned)
            {
                throw new InvalidOperationException("Cannot delete permission that is assigned to roles. Remove it from all roles first.");
            }

            _uow.Permissions.Remove(entity);
            await _uow.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            var permissions = await _uow.Permissions.GetAllAsync();
            return permissions.Any(p => p.Code != null && p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var entities = await _uow.Permissions.GetAllAsync();
            return entities.Select(p => _mapper.Map(p));
        }

        
        

        public async Task UpdateAsync(PermissionDto dto)
        {
            var entity = await _uow.Permissions.Query().FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (entity == null)
                throw new InvalidOperationException($"Permission with ID {dto.Id} not found.");

            // Check if code is being changed and if new code already exists
            if (!string.IsNullOrEmpty(dto.Code) && 
                !string.IsNullOrEmpty(entity.Code) &&
                !entity.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase) &&
                await ExistsAsync(dto.Code))
            {
                throw new InvalidOperationException($"Permission with code '{dto.Code}' already exists.");
            }

            entity.Code = dto.Code;
            entity.Description = dto.Description;
            _uow.Permissions.Update(entity);
            await _uow.SaveChangesAsync();
        }
    }
}
