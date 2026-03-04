using Microsoft.Extensions.Logging;
using UsersAPI.Domain.Common;

namespace UsersAPI.Infrastructure.Logging
{
    public class BaseLogger<T> : IAppLogger<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICorrelationIdGenerator _correlationId;


        public BaseLogger(ILogger<T> logger, ICorrelationIdGenerator correlationId)
        {
            _logger = logger;
            _correlationId = correlationId;
        }

        public virtual void LogInformation(string message)
        {
            _logger.LogInformation("{Message} | CorrelationId={CorrelationId}", message, _correlationId.Get());
        }

        public virtual void LogError(string message)
        {
            _logger.LogError("{Message} | CorrelationId={CorrelationId}", message, _correlationId.Get());
        }

        public virtual void LogWarning(string message)
        {
            _logger.LogWarning("{Message} | {CorrelationId={CorrelationId}", message, _correlationId.Get());
        }
    }
}
