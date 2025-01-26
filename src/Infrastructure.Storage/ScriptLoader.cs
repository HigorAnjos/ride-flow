using Application.Abstractions.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    [ExcludeFromCodeCoverage]
    public class ScriptLoader : IScriptLoader
    {
        private IMemoryCache _memoryCache { get; set; }

        public ScriptLoader(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<string> GetCachedScriptAsync(string scriptFolderPath, string scriptName)
        {
            return await _memoryCache.GetOrCreate($"{scriptFolderPath}.{scriptName}", async entry =>
            {
                entry.AbsoluteExpiration = DateTime.Today.AddDays(1);
                entry.SetPriority(CacheItemPriority.Normal);
                var script = await GetAsync(scriptFolderPath, scriptName);
                return script ?? string.Empty;
            });
        }


        public static async Task<string?> GetAsync(string scriptFolderPath, string scriptName)
            => await GetAsync(scriptFolderPath, scriptName, Encoding.Default);

        public static async Task<string?> GetAsync(string scriptFolderPath, string scriptName, Encoding encoding)
        {
            var executingAssembly = Assembly.GetAssembly(typeof(ScriptLoader));
            if (executingAssembly is null) return null;

            await using var stream = executingAssembly.GetManifestResourceStream($"{scriptFolderPath}.{scriptName}");
            if (stream is null) return null;

            using var reader = new StreamReader(stream, encoding);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
