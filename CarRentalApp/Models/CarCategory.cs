using CarRentalApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarRentalApp.Models
{
    public class CarCategory
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }

}
