using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Dtos.Common;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly IUserService _userService;

        public UsersController(
            UserManager<User> userManager, 
            RoleManager<AspNetRole> roleManager,
            IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        [HttpPost("/roles")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {dto.UserId} not found." });
            }

            var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {dto.RoleId} not found." });
            }

            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
            }

            return Ok(new { message = "Role assigned successfully." });
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {roleId} not found." });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = $"Failed to remove role: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
            }

            return Ok(new { message = "Role removed successfully." });
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleDtos = new List<RoleDto>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                    });
                }
            }

            return Ok(roleDtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PagedRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.GetAllUsersAsync(request);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            return Ok(user);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            // Prevent users from deleting themselves
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(currentUserIdClaim) && 
                Guid.TryParse(currentUserIdClaim, out var currentUserId) && 
                currentUserId == userId)
            {
                return BadRequest(new { message = "You cannot delete your own account." });
            }

            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
            {
                return NotFound(new { message = $"User with ID {userId} not found or already deleted." });
            }

            return Ok(new { message = "User deleted successfully." });
        }

    }
}
