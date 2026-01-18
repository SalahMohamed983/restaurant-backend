using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ResturantDataAccessLayer.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? ImageUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int TokenVersion { get; set; }

        // Navigation
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<MenuCategory>? CreatedMenuCategories { get; set; }
        public ICollection<MenuItem>? CreatedMenuItems { get; set; }
        public ICollection<DiningTable>? CreatedDiningTables { get; set; }
    }
}
