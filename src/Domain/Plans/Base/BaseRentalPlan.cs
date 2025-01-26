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

        public decimal CalculateTotalCost(int rentalDays, DateTime expectedEndDate, DateTime? returnDate)
        {
            if (rentalDays <= 0)
                return 0;

            return rentalDays * DailyRate + CalculatePenalty(expectedEndDate, returnDate);
        }

        public decimal CalculatePenalty(DateTime expectedEndDate, DateTime? returnDate)
        {
            if (!returnDate.HasValue)
                return 0;

            DateTime returnDateValue = returnDate.Value.Date;

            if (returnDateValue.Date < expectedEndDate.Date)
                return CalculateEarlyReturnPenalty(expectedEndDate, returnDateValue);

            if (returnDateValue.Date > expectedEndDate.Date)
                return CalculateLateReturnPenalty(expectedEndDate, returnDateValue);

            return 0;
        }

        protected abstract decimal CalculateEarlyReturnPenalty(DateTime expectedEndDate, DateTime returnDate);

        protected virtual decimal CalculateLateReturnPenalty(DateTime expectedEndDate, DateTime returnDate)
        {
            var additionalDays = (returnDate - expectedEndDate).Days;
            return additionalDays > 0 ? additionalDays * 50m : 0;
        }
    }
}
