using Domain.Entities;
using Domain.Plans.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Plans
{
    public class SevenDaysPlan : BaseRentalPlan
    {
        public SevenDaysPlan() : base(durationInDays: 7, dailyRate: 30m) { } // Duração de 7 dias, R$30 por dia

        protected override decimal CalculateEarlyReturnPenalty(Rental rental)
        {
            var daysNotUsed = (rental.ExpectedEndDate - rental.ReturnDate.Value).Days;
            return daysNotUsed * DailyRate * 0.2m; // Multa de 20% por dia não utilizado
        }
    }
}
