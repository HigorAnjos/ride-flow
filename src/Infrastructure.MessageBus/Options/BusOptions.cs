using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MessageBus.Options
{
    [ExcludeFromCodeCoverage]
    public abstract record BusOptions
    {
        [Required] public Uri ConnectionString { get; init; }
        [Required, Range(1, 10)] public int RetryLimit { get; init; }
        [Required, Timestamp] public TimeSpan InitialInterval { get; init; }
        [Required, Timestamp] public TimeSpan IntervalIncrement { get; init; }
        public string[]? Cluster { get; init; }
    }
}
