using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarRentalApp.Models;
using Xunit;

namespace CarRentalApp.Tests
{
    public class CarModelTests
    {
        // 1. Test walidacji modelu CAR
        [Theory]
        [InlineData("", "Corolla", 2022, 100)] 
        [InlineData("Toyota", "Corolla", 1800, 100)] 
        [InlineData("Toyota", "Corolla", 2022, -10)] 
        public void Car_ShouldReturnValidationError_ForInvalidData(string make, string model, int year, int price)
        {
            var car = new Car { Make = make, Model = model, Year = year, PricePerDay = price };
            var context = new ValidationContext(car);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(car, context, results, true);

            Assert.False(isValid);
        }

        // 2. Test w�a�ciwo�ci wyliczanej FullName
        [Fact]
        public void Car_FullName_ShouldReturnCorrectFormat()
        {
            var car = new Car { Make = "Toyota", Model - "Corolla", Year = 2022, PricePerDay = 150 };

            var fullName = car.FullName;

            Assert.Equal("Toyota Corolla (2022) - 150 zł/dzień", fullName);
        }

        // 3. Test walidacji modelu RESERVATION (Logika dat)
        [Fact]
        public void Reservation_Validation_ShouldPass_ForCorrectDates()
        {
            var reservation = new Reservation
            {
                CarId = 1,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(3)
            };

            var context = new ValidationContext(reservation);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(reservation, context, results, true);

            Assert.True(isValid);
        }

        // 4. Test walidacji modelu CAR CATEGORY
        [Fact]
        public void CarCategory_ShouldFail_WhenNameIsTooLong()
        {
            var category = new CarCategory { Name = new string('A', 51) }; 
            var context = new ValidationContext(category);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);

            Assert.False(isValid);
        }
    }
}



