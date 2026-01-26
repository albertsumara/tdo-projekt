using CarRentalApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        public Car? Car { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }

}
