using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Menu.Categories;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IMenuCategoryService
    {
        Task<MenuCategoryDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuCategoryDto>> GetAllAsync();
        Task<Guid> CreateAsync(MenuCategoryDto dto);
        Task UpdateAsync(MenuCategoryDto dto);
        Task DeleteAsync(Guid id);
    }
}
