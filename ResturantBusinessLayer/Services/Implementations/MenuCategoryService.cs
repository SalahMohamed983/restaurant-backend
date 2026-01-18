using ResturantBusinessLayer.Dtos.Menu.Categories;
using ResturantBusinessLayer.Mappers;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public MenuCategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(MenuCategoryDto dto)
        {
            var entity = _mapper.Map(dto);
            entity.Id = Guid.NewGuid();
            await _uow.MenuCategories.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _uow.MenuCategories.GetByIdAsync(id);
            if (e == null) return;
       
            e.IsActive = false;

            _uow.MenuCategories.Update(e);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuCategoryDto>> GetAllAsync()
        {
            var entities = await _uow.MenuCategories.GetAllAsync();
            return entities.Select(c => _mapper.Map(c));
        }

        public async Task<MenuCategoryDto?> GetByIdAsync(Guid id)
        {
            var c = await _uow.MenuCategories.GetByIdAsync(id);
            if (c == null) return null;
            return _mapper.Map(c);
        }

        public async Task UpdateAsync(MenuCategoryDto dto)
        {
            var e = await _uow.MenuCategories.GetByIdAsync(dto.Id);
            if (e == null) return;
            var updated = _mapper.Map(dto);
            e.Name = updated.Name;
            e.Description = updated.Description;
            e.IsActive = updated.IsActive;
            e.SortOrder = updated.SortOrder;
            _uow.MenuCategories.Update(e);
            await _uow.SaveChangesAsync();
        }
    }
}
