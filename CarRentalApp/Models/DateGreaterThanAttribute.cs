using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Jeśli brak instancji obiektu (np. podczas EF Core design-time), pomijamy walidację
            if (validationContext.ObjectInstance == null)
                return ValidationResult.Success;

            var currentValue = value as DateTime?;
            if (currentValue == null)
                return ValidationResult.Success;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return new ValidationResult($"Nie znaleziono właściwości {_comparisonProperty}");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateTime?;
            if (comparisonValue == null)
                return ValidationResult.Success;

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} musi być po {_comparisonProperty}");

            return ValidationResult.Success;
        }
    }
}
