using Xunit;
using CarRentalApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Tests
{
    public class CarModelTests
    {
        [Fact]
        public void Car_FullName_ShouldReturnCorrectFormat()
        {
            // Arrange
            var car = new Car
            {
                Id = 1,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2022,
                PricePerDay = 150
            };

            // Act
            string fullName = car.FullName;

            // Assert
            Assert.Equal("Toyota Corolla (2022) – 150 zł/dzień", fullName);
        }

        [Fact]
        public void Car_Validation_ShouldFail_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var car = new Car
            {
                Make = "", 
                Model = "" 
            };

            var context = new ValidationContext(car);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(car, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Make"));
            Assert.Contains(results, r => r.MemberNames.Contains("Model"));
        }

        [Fact]
        public void Car_ShouldInitialize_ReservationsList()
        {
            var car = new Car();

            Assert.NotNull(car.Reservations);
            Assert.Empty(car.Reservations);
        }
    }
}
