using System;

namespace Application.Abstractions.Logging
{
    public interface ILoggerService<T>
    {
        /// <summary>
        /// Registra uma mensagem de log com nível de informação.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        void LogInformation(string message);

        /// <summary>
        /// Registra uma mensagem de log com nível de informação, com parâmetros estruturados.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        /// <param name="args">Parâmetros da mensagem.</param>
        void LogInformation(string message, params object[] args);

        /// <summary>
        /// Registra uma mensagem de log com nível de aviso.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        void LogWarning(string message);

        /// <summary>
        /// Registra uma mensagem de log com nível de aviso, com parâmetros estruturados.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        /// <param name="args">Parâmetros da mensagem.</param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// Registra uma mensagem de log com nível de erro.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        void LogError(string message);

        /// <summary>
        /// Registra uma mensagem de log com nível de erro, com uma exceção e parâmetros estruturados.
        /// </summary>
        /// <param name="exception">Exceção associada ao erro.</param>
        /// <param name="message">Mensagem a ser registrada.</param>
        /// <param name="args">Parâmetros da mensagem.</param>
        void LogError(Exception exception, string message, params object[] args);
    }
}
