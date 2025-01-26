using Application.Abstractions.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.RideFlowBus.Logging
{
    public class LoggerService<T> : ILoggerService<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerService(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }
    }
}
