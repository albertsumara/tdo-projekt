using CarRentalApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
