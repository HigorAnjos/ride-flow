using Application.Abstractions.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.UseCase.Base
{
    [ExcludeFromCodeCoverage]
    public class BaseUseCase<T>
    {
        private readonly ILoggerService<T> _logger;

        protected virtual string ActionIdentification { get; } = nameof(T);

        protected BaseUseCase(ILoggerService<T> logger)
        {
            _logger = logger;
        }

        protected void LogInformation(string message)
        {
            _logger.LogInformation($"{ActionIdentification} - {message}");
        }

        protected void LogInformation(string message, params object[] @params)
        {
            _logger.LogInformation($"{ActionIdentification} - {message}", @params);
        }

        protected void LogWarning(string message)
        {
            _logger.LogWarning($"{ActionIdentification} - {message}");
        }

        protected void LogWarning(string message, params object[] @params)
        {
            _logger.LogWarning($"{ActionIdentification} - {message}", @params);
        }

        protected void LogError(string message)
        {
            _logger.LogError($"{ActionIdentification} - {message}");
        }

        protected void LogError(Exception ex, string message, params object[] @params)
        {
            _logger.LogError(ex, $"{ActionIdentification} - {message}", @params);
        }
    }
}
