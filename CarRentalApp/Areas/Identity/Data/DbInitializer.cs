using CarRentalApp.Areas.Identity.Data;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            var context = serviceProvider.GetRequiredService<AppDbContext>();
            //await context.Database.MigrateAsync();


            await TworzenieAdmina.InicjalizujRole(serviceProvider);
            await TworzenieAdmina.InicjalizujKategorie(serviceProvider);

            //var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            if (!userManager.Users.Any(u => u.UserName.StartsWith("user")))
            {
                for (int i = 1; i <= 5; i++)
                {
                    var user = new ApplicationUser
                    {
                        UserName = $"user{i}@example.com",
                        Email = $"user{i}@example.com",
                        FirstName = $"User{i}",
                        LastName = "Testowy",
                        EmailConfirmed = true,
                        IsAdmin = false
                    };

                    await userManager.CreateAsync(user, $"User{i}!123");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }


            if (!context.Cars.Any())
            {
                var sedanCategory = context.CarCategories.First(c => c.Name == "Sedan");
                var suvCategory = context.CarCategories.First(c => c.Name == "SUV");
                var kombiCategory = context.CarCategories.First(c => c.Name == "Kombi");
                var hatchbackCategory = context.CarCategories.First(c => c.Name == "Hatchback");
                var sportCategory = context.CarCategories.First(c => c.Name == "Sportowy");

                context.Cars.AddRange(
                    new Car { Make = "Toyota", Model = "Corolla", Year = 2020, PricePerDay = 150, CarCategoryId = sedanCategory.Id },
                    new Car { Make = "BMW", Model = "X5", Year = 2022, PricePerDay = 400, CarCategoryId = suvCategory.Id },
                    new Car { Make = "Audi", Model = "A3", Year = 2019, PricePerDay = 200, CarCategoryId = sedanCategory.Id },
                    new Car { Make = "Mercedes", Model = "C200", Year = 2021, PricePerDay = 250, CarCategoryId = sedanCategory.Id },
                    new Car { Make = "Volkswagen", Model = "Golf", Year = 2018, PricePerDay = 120, CarCategoryId = hatchbackCategory.Id },
                    new Car { Make = "Porsche", Model = "911", Year = 2022, PricePerDay = 800, CarCategoryId = sportCategory.Id },
                    new Car { Make = "Ford", Model = "Focus Kombi", Year = 2019, PricePerDay = 130, CarCategoryId = kombiCategory.Id },
                    new Car { Make = "Nissan", Model = "Qashqai", Year = 2021, PricePerDay = 180, CarCategoryId = suvCategory.Id }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Reservations.Any())
            {
                var users = userManager.Users.ToList();
                var cars = context.Cars.ToList();

                var rnd = new Random();
                foreach (var user in users)
                {
                    var car = cars[rnd.Next(cars.Count)];

                    context.Reservations.Add(new Reservation
                    {
                        CarId = car.Id,
                        UserId = user.Id,
                        StartDate = DateTime.SpecifyKind(DateTime.Today.AddDays(rnd.Next(1, 10)), DateTimeKind.Utc),
                        EndDate = DateTime.SpecifyKind(DateTime.Today.AddDays(rnd.Next(11, 20)), DateTimeKind.Utc)
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
