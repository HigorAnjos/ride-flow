using Domain.Enumerables;
using Domain.Plans.Base;

namespace Domain.Plans
{
    public interface IRentalPlanFactory
    {
        IRentalPlan Create(RentalPlanTypeEnum planType);
    }

    public class RentalPlanFactory : IRentalPlanFactory
    {
        public IRentalPlan Create(RentalPlanTypeEnum planType)
        {
            return planType switch
            {
                RentalPlanTypeEnum.SevenDays => new SevenDaysPlan(),
                RentalPlanTypeEnum.FifteenDays => new FifteenDaysPlan(),
                RentalPlanTypeEnum.ThirtyDays => new ThirtyDaysPlan(),
                RentalPlanTypeEnum.FortyFiveDays => new FortyFiveDaysPlan(),
                RentalPlanTypeEnum.FiftyDays => new FiftyDaysPlan(),
                _ => throw new InvalidDataException("Plano de locação inválido.")
            };
        }
    }
}
