using Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        public IDbConnection Connection { get; }

        public IDbTransaction Transaction { get; set; }

        public UnitOfWork(IDbConnection dbConnection)
        {
            Connection = dbConnection;
        }

        public async Task ExecuteTransaction(Func<CancellationToken, Task> funcAsync, CancellationToken cancellationToken = default)
        {
            if (Transaction != null)
            {
                await funcAsync(cancellationToken);
            }
            else
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                using (Transaction = Connection.BeginTransaction())
                {
                    await funcAsync(cancellationToken);
                    Transaction.Commit();
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (Transaction != null)
                Transaction.Dispose();

            Transaction = null;

            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }
    }
}
