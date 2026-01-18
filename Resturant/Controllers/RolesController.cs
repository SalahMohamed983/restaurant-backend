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
    //[Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IRolePermissionService _rolePermissionService;

        public RolesController(IRoleService roleService, IRolePermissionService rolePermissionService)
        {
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("with-permissions")]
        public async Task<IActionResult> GetAllWithPermissions()
        {
            var roles = await _roleService.GetAllWithPermissionsAsync();
            return Ok(roles);
        }


        [HttpGet("{id}/with-permissions")]
        public async Task<IActionResult> GetByIdWithPermissions(Guid id)
        {
            var role = await _roleService.GetByIdWithPermissionsAsync(id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var roleId = await _roleService.CreateAsync(roleDto);
                return Ok( new { id = roleId, message = "Role created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update( [FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          

            try
            {
                await _roleService.UpdateAsync(roleDto);
                return Ok(new { message = "Role updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _roleService.DeleteAsync(id);
                return Ok(new { message = "Role deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(Guid id)
        {
            var permissions = await _rolePermissionService.GetPermissionsByRoleAsync(id);
            return Ok(permissions);
        }

        
        [HttpPost("permissions/multiple")]
        public async Task<IActionResult> AssignMultiplePermissions([FromBody] AssignMultiplePermissionsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

               try
            {
                await _rolePermissionService.AssignMultiplePermissionsAsync(dto);
                return Ok(new { message = "Permissions assigned successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/permissions/{permissionId}")]
        public async Task<IActionResult> RemovePermission(Guid id, int permissionId)
        {
            // remove a specific permission from a role
            try
            {
                var rolePermissionDto = new RolePermissionDto
                {
                    RoleId = id,
                    PermissionId = permissionId
                };
                await _rolePermissionService.RemovePermissionAsync(rolePermissionDto);
                return Ok(new { message = "Permission removed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/permissions")]
        public async Task<IActionResult> RemoveAllPermissions(Guid id)
        {
            // remove all permissions from a role
            try
            {
                await _rolePermissionService.RemoveAllPermissionsAsync(id);
                return Ok(new { message = "All permissions removed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
