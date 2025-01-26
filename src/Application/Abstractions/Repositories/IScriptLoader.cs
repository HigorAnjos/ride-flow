namespace Application.Abstractions.Repositories
{
    public interface IScriptLoader
    {
        public Task<string?> GetCachedScriptAsync(string scriptFolderPath, string scriptName);
    }
}
