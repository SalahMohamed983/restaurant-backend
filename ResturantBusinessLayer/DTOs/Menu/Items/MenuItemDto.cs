using System;

namespace ResturantBusinessLayer.Dtos.Menu.Items
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
