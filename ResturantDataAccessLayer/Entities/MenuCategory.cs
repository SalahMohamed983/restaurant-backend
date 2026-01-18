using System;
using System.Collections.Generic;

namespace ResturantDataAccessLayer.Entities
{
    public class MenuCategory
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public User? Creator { get; set; }
        public User? Updater { get; set; }
        public ICollection<MenuItem>? MenuItems { get; set; }
    }
}
