using Domain.Entities;
using Domain.Enumerables;
using Domain.Plans;
using FluentAssertions;

namespace UnitTests.Domain.Entities
{
    public class RentalUnitTest
    {
        [Fact]
        public void SetRentalPlan_ShouldSetPlanCorrectly()
        {
            // Arrange
            var rental = new Rental
            {
                Id = "test-id",
                DeliveryPersonId = "delivery-id",
                MotorcycleId = "motorcycle-id"
            };

            var rentalPlan = new SevenDaysPlan();

            // Act
            rental.SetRentalPlan(rentalPlan, RentalPlanTypeEnum.SevenDays);

            // Assert
            rental.RentalPlan.Should().Be(rentalPlan);
            rental.RentalPlanType.Should().Be(RentalPlanTypeEnum.SevenDays);
        }

        [Fact]
        public void SetRentalPlan_ShouldThrowException_WhenPlanIsNull()
        {
            // Arrange
            var rental = new Rental();

            // Act
            Action act = () => rental.SetRentalPlan(null, RentalPlanTypeEnum.SevenDays);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("O plano de locação não pode ser nulo. (Parameter 'rentalPlan')");
        }

        [Fact]
        public void SetRentalPlan_ShouldThrowException_WhenPlanTypeIsInvalid()
        {
            // Arrange
            var rental = new Rental();
            var rentalPlan = new SevenDaysPlan();

            // Act
            Action act = () => rental.SetRentalPlan(rentalPlan, (RentalPlanTypeEnum)999);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Tipo de plano de locação inválido. (Parameter 'rentalPlanType')");
        }

        [Fact]
        public void SetReturnDate_ShouldSetCorrectReturnDate()
        {
            // Arrange
            var rental = new Rental
            {
                StartDate = new DateTime(2024, 1, 1)
            };

            var returnDate = new DateTime(2024, 1, 5);

            // Act
            rental.SetReturnDate(returnDate);

            // Assert
            rental.ReturnDate.Should().Be(returnDate);
        }

        [Fact]
        public void SetReturnDate_ShouldThrowException_WhenReturnDateIsBeforeStartDate()
        {
            // Arrange
            var rental = new Rental
            {
                StartDate = new DateTime(2024, 1, 1)
            };

            var returnDate = new DateTime(2023, 12, 31);

            // Act
            Action act = () => rental.SetReturnDate(returnDate);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("A data de devolução não pode ser anterior à data de início.");
        }

        [Fact]
        public void CalculateFinalCost_ShouldReturnCorrectValue()
        {
            // Arrange
            var rental = new Rental();

            var rentalPlan = new SevenDaysPlan();
            rental.SetRentalPlan(rentalPlan, RentalPlanTypeEnum.SevenDays);
            rental.SetReturnDate(DateTime.Now.AddDays(6)); // Alugado por 5 dias

            // Act
            var totalCost = rental.CalculateFinalCost();

            // Assert
            totalCost.Should().Be(162); 
        }


        [Fact]
        public void CalculatePenalty_ShouldReturnZero_WhenNoPenaltyIsApplied()
        {
            // Arrange
            var rental = new Rental
            {
                StartDate = new DateTime(2024, 1, 1),
                ReturnDate = (DateTime?)new DateTime(2024, 1, 9)
            };

            var rentalPlan = new SevenDaysPlan();
            rental.SetRentalPlan(rentalPlan, RentalPlanTypeEnum.SevenDays);
            rental.SetReturnDate(DateTime.Now.AddDays(8));

            // Act
            var penalty = rental.CalculatePenalty();

            // Assert
            penalty.Should().Be(0);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
        {
            // Arrange
            var rental = new Rental
            {
                Id = "test-id",
                DeliveryPersonId = "delivery-id",
                MotorcycleId = "motorcycle-id",
                StartDate = new DateTime(2024, 1, 1),
            };

            var rentalPlan = new SevenDaysPlan();
            rental.SetRentalPlan(rentalPlan, RentalPlanTypeEnum.SevenDays);

            // Act
            var isValid = rental.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenPropertiesAreInvalid()
        {
            // Arrange
            var rental = new Rental
            {
                Id = string.Empty,
                DeliveryPersonId = null,
                MotorcycleId = null,
                StartDate = new DateTime(2024, 1, 1),
            };

            // Act
            var isValid = rental.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
