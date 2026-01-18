using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Menu.Items;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<MenuItemDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuItemDto>> GetAllAsync();
        Task<Guid> CreateAsync(MenuItemDto dto);
        Task UpdateAsync(MenuItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
