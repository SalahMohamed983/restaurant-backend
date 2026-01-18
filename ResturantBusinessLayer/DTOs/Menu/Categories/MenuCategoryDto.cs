using System;

namespace ResturantBusinessLayer.Dtos.Menu.Categories
{
    public class MenuCategoryDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
    }
}
