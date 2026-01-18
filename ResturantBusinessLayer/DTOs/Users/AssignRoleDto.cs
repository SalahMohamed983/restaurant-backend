using System;
using System.ComponentModel.DataAnnotations;

namespace ResturantBusinessLayer.Dtos.Users
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Role ID is required")]
        public Guid RoleId { get; set; }
    }

    public class AssignMultiplePermissionsDto
    {
        [Required(ErrorMessage = "Role ID is required")]
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "Permission IDs are required")]
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
