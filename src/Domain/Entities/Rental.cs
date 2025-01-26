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
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public RentalPlanTypeEnum RentalPlanType { get; set; }

        [NotMapped]
        public IRentalPlan RentalPlan { get; set; }

        public void SetRentalPlan(IRentalPlan rentalPlan, RentalPlanTypeEnum rentalPlanType)
        {
            if (rentalPlan == null)
                throw new ArgumentNullException(nameof(rentalPlan), "O plano de locação não pode ser nulo.");

            if (!Enum.IsDefined(typeof(RentalPlanTypeEnum), rentalPlanType))
                throw new ArgumentException("Tipo de plano de locação inválido.", nameof(rentalPlanType));

            RentalPlan = rentalPlan;
            RentalPlanType = rentalPlanType;
            StartDate = DateTime.Now.AddDays(1);
            EndDate = StartDate.AddDays(rentalPlan.DurationInDays);
            ExpectedEndDate = EndDate;
        }

        public void SetReturnDate(DateTime returnDate)
        {
            if (returnDate < StartDate)
                throw new ArgumentException("A data de devolução não pode ser anterior à data de início.");

            ReturnDate = returnDate;
        }

        public decimal CalculateFinalCost()
        {
            if (RentalPlan == null)
                throw new InvalidOperationException("O plano de locação não foi definido.");

            var rentalDays = (ReturnDate?.Date - StartDate.Date)?.Days ?? 0;

            return RentalPlan.CalculateTotalCost(rentalDays, ExpectedEndDate, ReturnDate);
        }

        public decimal CalculatePenalty()
        {
            if (RentalPlan == null)
                throw new InvalidOperationException("O plano de locação não foi definido.");

            return RentalPlan.CalculatePenalty(ExpectedEndDate, ReturnDate);
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
