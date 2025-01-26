using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; set; }

        Task ExecuteTransaction(Func<CancellationToken, Task> funcAsync, CancellationToken cancellationToken);
    }
}
