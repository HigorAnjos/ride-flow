using Domain.Entities;
using Domain.Plans.Base;

namespace Domain.Plans
{
    public class FortyFiveDaysPlan : BaseRentalPlan
    {
        public FortyFiveDaysPlan() : base(durationInDays: 45, dailyRate: 20.00m) { }

        protected override decimal CalculateEarlyReturnPenalty(Rental rental)
        {
            // Penalidade específica para devolução antecipada (a ser definida no futuro)
            return 0;
        }
    }
}
