using System;

namespace ResturantDataAccessLayer.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public MenuCategory? Category { get; set; }
        public User? Creator { get; set; }
        public User? Updater { get; set; }
    }
}
