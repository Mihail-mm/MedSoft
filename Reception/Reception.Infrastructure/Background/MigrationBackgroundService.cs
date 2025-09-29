using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reception.Infrastructure.Background;

public class MigrationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrationBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrationUp();
    }
    
    private async Task MigrationUp()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}