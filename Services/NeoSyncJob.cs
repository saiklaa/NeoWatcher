public class NeoSyncJob : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<NeoSyncJob> _logger;

    public NeoSyncJob(IServiceProvider provider, ILogger<NeoSyncJob> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var sync = scope.ServiceProvider.GetRequiredService<NeoSyncService>();

            try
            {
                await sync.FetchAndSyncAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync NASA NEO feed.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}