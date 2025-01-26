using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Storage.Repositories.Base
{
    [ExcludeFromCodeCoverage]
    public abstract class RepositoryBase
    {
        protected abstract string FolderPath { get; }

        protected RepositoryBase() { }
    }
}
