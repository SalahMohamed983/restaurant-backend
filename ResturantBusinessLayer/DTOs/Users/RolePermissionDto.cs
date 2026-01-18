using System;

namespace ResturantBusinessLayer.Dtos.Users
{
    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public int PermissionId { get; set; }
    }
}
