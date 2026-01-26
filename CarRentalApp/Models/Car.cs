using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Make { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Range(0, 10000)]
        public int PricePerDay { get; set; }

        public int CarCategoryId { get; set; }
        public CarCategory? CarCategory { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        [NotMapped]
        public string FullName => $"{Make} {Model} ({Year}) – {PricePerDay} zł/dzień";
    }
}
