using System;

namespace ResturantDataAccessLayer.Entities
{
    public class DiningTable
    {
        public Guid Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public User? Creator { get; set; }
        public User? Updater { get; set; }
    }
}
