using System;

namespace ResturantBusinessLayer.Dtos.Reservations.Tables
{
    public class DiningTableDto
    {
        public Guid Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }
}
