using Domain.Entities;

namespace Domain.Plans.Base
{
    public interface IRentalPlan
    {
        public int DurationInDays { get; } // Duração do plano em dias
        public decimal DailyRate { get; } // Valor diário do plano
        public decimal CalculatePenalty(Rental rental); // Cálculo da penalidade
        public decimal CalculateTotalCost(int rentalDays); // Cálculo do custo total
    }
}
