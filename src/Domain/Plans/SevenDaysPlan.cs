using Domain.Plans.Base;

namespace Domain.Plans
{
    public class SevenDaysPlan : BaseRentalPlan
    {
        public SevenDaysPlan() : base(durationInDays: 7, dailyRate: 30m) { }

        protected override decimal CalculateEarlyReturnPenalty(DateTime expectedEndDate, DateTime returnDate)
        {
            var daysNotUsed = (expectedEndDate - returnDate).Days;
            return daysNotUsed * DailyRate * 0.2m;
        }
    }
}
