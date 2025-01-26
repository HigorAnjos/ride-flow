using Domain.Entities;

namespace Domain.Plans.Base
{
    public interface IRentalPlan
    {
        public int DurationInDays { get; } // Duração do plano em dias
        public decimal DailyRate { get; } // Valor diário do plano
        public decimal CalculateTotalCost(int rentalDays, DateTime expectedEndDate, DateTime? returnDate);
        public decimal CalculatePenalty(DateTime expectedEndDate, DateTime? returnDate);
    }
}
