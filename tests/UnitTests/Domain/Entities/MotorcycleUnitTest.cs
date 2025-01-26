using System;
using Domain.Entities;
using Xunit;
using FluentAssertions;

namespace UnitTests.Domain.Entities
{
    public class MotorcycleUnitTest
    {
        [Fact]
        public void IsFromYear_ShouldReturnTrue_WhenYearMatches()
        {
            // Arrange
            var motorcycle = new Motorcycle { Year = 2024 };

            // Act
            var result = motorcycle.IsFromYear(2024);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsFromYear_ShouldReturnFalse_WhenYearDoesNotMatch()
        {
            // Arrange
            var motorcycle = new Motorcycle { Year = 2023 };

            // Act
            var result = motorcycle.IsFromYear(2024);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void UpdateLicensePlate_ShouldUpdate_WhenValidLicensePlateProvided()
        {
            // Arrange
            var motorcycle = new Motorcycle();
            var newLicensePlate = "ABC-1234";

            // Act
            motorcycle.UpdateLicensePlate(newLicensePlate);

            // Assert
            motorcycle.LicensePlate.Should().Be(newLicensePlate);
        }

        [Fact]
        public void UpdateLicensePlate_ShouldThrowException_WhenLicensePlateIsEmpty()
        {
            // Arrange
            var motorcycle = new Motorcycle();

            // Act
            Action act = () => motorcycle.UpdateLicensePlate("");

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("A placa não pode ser vazia.");
        }

        [Fact]
        public void UpdateLicensePlate_ShouldThrowException_WhenLicensePlateIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle();
            var invalidLicensePlate = "123-ABCD";

            // Act
            Action act = () => motorcycle.UpdateLicensePlate(invalidLicensePlate);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("A placa fornecida é inválida. Use o formato ABC-1234.");
        }

        [Fact]
        public void IsValidLicensePlate_ShouldReturnTrue_WhenLicensePlateIsValid()
        {
            // Arrange
            var motorcycle = new Motorcycle();
            var validLicensePlate = "XYZ-9876";

            // Act
            var result = motorcycle.IsValidLicensePlate(validLicensePlate);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidLicensePlate_ShouldReturnFalse_WhenLicensePlateIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle();
            var invalidLicensePlate = "9876-XYZ";

            // Act
            var result = motorcycle.IsValidLicensePlate(invalidLicensePlate);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = "1",
                Year = 2023,
                Model = "Sport",
                LicensePlate = "DEF-5678"
            };

            // Act
            var result = motorcycle.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenIdIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = "",
                Year = 2023,
                Model = "Sport",
                LicensePlate = "DEF-5678"
            };

            // Act
            var result = motorcycle.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenYearIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = "1",
                Year = 1800,
                Model = "Sport",
                LicensePlate = "DEF-5678"
            };

            // Act
            var result = motorcycle.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = "1",
                Year = 2023,
                Model = "",
                LicensePlate = "DEF-5678"
            };

            // Act
            var result = motorcycle.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenLicensePlateIsInvalid()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = "1",
                Year = 2023,
                Model = "Sport",
                LicensePlate = "1234-DEF"
            };

            // Act
            var result = motorcycle.IsValid();

            // Assert
            result.Should().BeFalse();
        }
    }
}
