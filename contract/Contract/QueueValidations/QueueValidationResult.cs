using MassTransit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.QueueValidations
{
    [ExcludeFromCodeCoverage]
    [ExcludeFromTopology]
    public abstract record QueueValidationResult<TMessage>(TMessage Message, IEnumerable<string> Errors);
}
