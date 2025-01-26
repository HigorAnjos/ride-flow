using Domain.Entities;
using Domain.Plans.Base;

namespace Domain.Plans
{
    public class FiftyDaysPlan : BaseRentalPlan
    {
        public FiftyDaysPlan() : base(durationInDays:50, dailyRate: 18.00m) { }

        protected override decimal CalculateEarlyReturnPenalty(Rental rental)
        {
            // Penalidade específica para devolução antecipada (a ser definida no futuro)
            return 0;
        }
    }
}
