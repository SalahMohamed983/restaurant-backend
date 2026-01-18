using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Menu.Items;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantBusinessLayer.Mappers;
using ResturantDataAccessLayer.UnitOfWork;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityMappers _mapper = new EntityMappers();

        public MenuItemService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(MenuItemDto dto)
        {
            var entity = _mapper.Map(dto);
            entity.Id = Guid.NewGuid();
            await _uow.MenuItems.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _uow.MenuItems.GetByIdAsync(id);
            if (e == null) return;
            _uow.MenuItems.Remove(e);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllAsync()
        {
            var entities = await _uow.MenuItems.GetAllAsync();
            return entities.Select(i => _mapper.Map(i));
        }

        public async Task<MenuItemDto?> GetByIdAsync(Guid id)
        {
            var i = await _uow.MenuItems.GetByIdAsync(id);
            if (i == null) return null;
            return _mapper.Map(i);
        }

        public async Task UpdateAsync(MenuItemDto dto)
        {
            var e = await _uow.MenuItems.GetByIdAsync(dto.Id);
            if (e == null) return;
            var updated = _mapper.Map(dto);
           
            e.CategoryId = updated.CategoryId;
            e.Name = updated.Name;
            e.Description = updated.Description;
            e.Price = updated.Price;
            e.ImageUrl = updated.ImageUrl;
            e.IsAvailable = updated.IsAvailable;
            _uow.MenuItems.Update(e);
            await _uow.SaveChangesAsync();
        }
    }
}
