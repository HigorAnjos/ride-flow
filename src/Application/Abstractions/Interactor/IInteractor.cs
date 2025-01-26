using Contract.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Interactor
{
    public interface IInteractor<in TMessage, TResult>
        where TMessage : IMessage
    {
        Task<TResult> InteractAsync(TMessage message, CancellationToken cancellationToken);
    }
}
