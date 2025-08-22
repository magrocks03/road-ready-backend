using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        public string StoreName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        // Navigation property: A location can have many vehicles.
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
