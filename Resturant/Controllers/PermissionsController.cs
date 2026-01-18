using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllAsync();
            return Ok(permissions);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var permissionId = await _permissionService.CreateAsync(permissionDto);
                return Ok(new { id = permissionId, message = "Permission created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permissionDto.Id)
            {
                return BadRequest(new { message = "Permission ID mismatch." });
            }

            try
            {
                await _permissionService.UpdateAsync(permissionDto);
                return Ok(new { message = "Permission updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _permissionService.DeleteAsync(id);
                return Ok(new { message = "Permission deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
