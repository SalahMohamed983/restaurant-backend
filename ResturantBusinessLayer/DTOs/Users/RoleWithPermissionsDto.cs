using System;
using System.Collections.Generic;

namespace ResturantBusinessLayer.Dtos.Users
{
    public class RoleWithPermissionsDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
