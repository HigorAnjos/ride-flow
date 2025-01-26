using Domain.Entities;
using Domain.Plans.Base;

namespace Domain.Plans
{
    public class FifteenDaysPlan : BaseRentalPlan
    {
        public FifteenDaysPlan() : base(durationInDays: 15, dailyRate: 28m) { } // Duração de 15 dias, R$28 por dia

        protected override decimal CalculateEarlyReturnPenalty(DateTime expectedEndDate, DateTime returnDate)
        {
            var daysNotUsed = (expectedEndDate - returnDate).Days;
            return daysNotUsed * DailyRate * 0.4m;
        }
    }
}
