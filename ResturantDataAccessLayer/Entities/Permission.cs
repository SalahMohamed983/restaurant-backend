using System;

namespace ResturantDataAccessLayer.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
}
