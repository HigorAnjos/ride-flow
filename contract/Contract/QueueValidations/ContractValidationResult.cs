using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.QueueValidations
{
    [ExcludeFromCodeCoverage]
    public record ContractValidationResult<TMessage>(TMessage Message, IEnumerable<string> Errors)
       : QueueValidationResult<TMessage>(Message, Errors);
}
