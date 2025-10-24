using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Reception.Application.Abstraction;
using Reception.Application.Models;
using Reception.Infrastructure.Background;
using Reception.Infrastructure.Repositories;

namespace Reception.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres;";

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        
        dataSourceBuilder.MapEnum<PatientStatus>("patient_status");

        var dataSource = dataSourceBuilder.Build();
        serviceCollection.AddSingleton(dataSource);

        serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ServiceCollectionExtensions).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        serviceCollection.AddScoped<IPatientRepository, PatientRepository>();
        serviceCollection.AddHostedService<MigrationBackgroundService>();

        return serviceCollection;
    }
}