using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions<T>(this IServiceCollection services, IConfigurationSection section)
           where T : class
           => services.AddOptions<T>()
                      .Bind(section) 
                      .ValidateDataAnnotations()
                      .ValidateOnStart()
                      .Services;
    }
}
