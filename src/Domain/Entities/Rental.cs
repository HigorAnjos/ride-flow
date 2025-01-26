using Domain.Enumerables;
using Domain.Plans.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Rental
    {
        public string Id { get; set; }
        public string DeliveryPersonId { get; set; }
        public string MotorcycleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; private set; }
        public DateTime ExpectedEndDate { get; private set; }
        public DateTime? ReturnDate { get; set; }
        public RentalPlanTypeEnum RentalPlanType { get; private set; }

        [NotMapped]
        public IRentalPlan RentalPlan { get; private set; }

        public void SetRentalPlan(IRentalPlan rentalPlan, RentalPlanTypeEnum rentalPlanType)
        {
            if (rentalPlan == null)
                throw new ArgumentNullException(nameof(rentalPlan), "O plano de locação não pode ser nulo.");

            if (!Enum.IsDefined(typeof(RentalPlanTypeEnum), rentalPlanType))
                throw new ArgumentException("Tipo de plano de locação inválido.", nameof(rentalPlanType));

            RentalPlan = rentalPlan;
            RentalPlanType = rentalPlanType;
            EndDate = StartDate.AddDays(rentalPlan.DurationInDays);
            ExpectedEndDate = EndDate;
        }

        public void SetReturnDate(DateTime returnDate)
        {
            if (returnDate < StartDate)
                throw new ArgumentException("A data de devolução não pode ser anterior à data de início.");

            ReturnDate = returnDate;
        }

        public decimal CalculateTotalCost()
        {
            if (RentalPlan == null)
                throw new InvalidOperationException("O plano de locação não foi definido.");

            var rentalDays = (EndDate - StartDate).Days + 1;
            return RentalPlan.CalculateTotalCost(rentalDays);
        }

        public decimal CalculatePenalty()
        {
            if (RentalPlan == null)
                throw new InvalidOperationException("O plano de locação não foi definido.");

            return RentalPlan.CalculatePenalty(this);
        }

        public decimal CalculateFinalCost()
        {
            return CalculateTotalCost() + CalculatePenalty();
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) &&
                   !string.IsNullOrWhiteSpace(DeliveryPersonId) &&
                   !string.IsNullOrWhiteSpace(MotorcycleId) &&
                   RentalPlan != null &&
                   Enum.IsDefined(typeof(RentalPlanTypeEnum), RentalPlanType) &&
                   (!ReturnDate.HasValue || ReturnDate >= StartDate);
        }
    }
}
