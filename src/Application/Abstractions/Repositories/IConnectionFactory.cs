using Application.Enumerations;
using System.Data;

namespace Application.Abstractions.Repositories
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection(Databases connectionDataBase);
    }
}
