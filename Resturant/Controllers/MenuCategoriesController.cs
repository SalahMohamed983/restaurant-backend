using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Dtos.Menu.Categories;
using ResturantBusinessLayer.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly IMenuCategoryService _categoryService;

        public MenuCategoriesController(IMenuCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        //[RequirePermission("GetCategories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var categoryId = await _categoryService.CreateAsync(categoryDto);
                return CreatedAtAction(nameof(GetById), new { id = categoryId }, new { id = categoryId, message = "Category created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update( [FromBody] MenuCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _categoryService.UpdateAsync(categoryDto);
                return Ok(new { message = "Category updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return Ok(new { message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
