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

        
        
    }
}
