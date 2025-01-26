using Domain.Entities;

namespace Domain.Plans.Base
{
    public abstract class BaseRentalPlan : IRentalPlan
    {
        public int DurationInDays { get; }
        public decimal DailyRate { get; }

        protected BaseRentalPlan(int durationInDays, decimal dailyRate)
        {
            DurationInDays = durationInDays;
            DailyRate = dailyRate;
        }

        public decimal CalculateTotalCost(int rentalDays)
        {
            return rentalDays * DailyRate;
        }

        public decimal CalculatePenalty(Rental rental)
        {
            if (rental.ReturnDate == null)
                return 0;

            if (rental.ReturnDate < rental.ExpectedEndDate)
                return CalculateEarlyReturnPenalty(rental);

            if (rental.ReturnDate > rental.ExpectedEndDate)
                return CalculateLateReturnPenalty(rental);

            return 0;
        }

        protected abstract decimal CalculateEarlyReturnPenalty(Rental rental); // Penalidade antecipada
        protected virtual decimal CalculateLateReturnPenalty(Rental rental) // Penalidade atrasada (padrão)
        {
            var additionalDays = (rental.ReturnDate.Value - rental.ExpectedEndDate).Days;
            return additionalDays * 50m; // Multa fixa de R$50 por dia
        }
    }
}
