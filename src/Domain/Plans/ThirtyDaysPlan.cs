using Domain.Entities;
using Domain.Plans.Base;

namespace Domain.Plans
{
    public class ThirtyDaysPlan : BaseRentalPlan
    {
        public ThirtyDaysPlan() : base(durationInDays: 30, dailyRate: 22.00m) { }

        protected override decimal CalculateEarlyReturnPenalty(Rental rental)
        {
            // Penalidade específica para devolução antecipada (a ser definida no futuro)
            return 0;
        }
    }
}
