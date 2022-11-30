using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace App
{
    public interface IWorker
    {
        Task DoWorkAsync(CancellationToken cancellationToken = default);
    }

    public class Worker : IWorker
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelemetryClient _client;

        public string WorkId { get; } = Guid.NewGuid().ToString();

        public Worker(ILogger<Worker> logger, TelemetryClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var operation = _client.StartOperation<RequestTelemetry>($"DoWork {WorkId}");
            using var scope = _logger.BeginScope(GetCustomProperties());
            _logger.LogInformation("Starting work {WorkId}", WorkId);
            await InvokeHttpDependencyAsync(cancellationToken);
            await InvokeQueueDependencyAsync(cancellationToken);
            _logger.LogInformation("Stopping work {WorkId}", WorkId);
        }

        private async Task InvokeHttpDependencyAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            _logger.LogTrace("Invoke http dependency {WorkId}", WorkId);
        }

        private async Task InvokeQueueDependencyAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            _logger.LogTrace("Invoke queue dependency {WorkId}", WorkId);
        }

        private static IDictionary<string, object> GetCustomProperties()
        {
            var customProperties = new Dictionary<string, object>
            {
                ["UserName"] = Environment.UserName,
                ["MachineName"] = Environment.MachineName
            };

            return customProperties;
        }
    }
}
