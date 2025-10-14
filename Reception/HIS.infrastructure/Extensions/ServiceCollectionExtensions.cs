using FluentMigrator.Runner;
using HIS.Application.Abstractions;
using HIS.infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace HIS.infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        var connectionString = "Host=localhost;Port=5430;Database=postgres;Username=postgres;Password=postgres;";

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

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

        return serviceCollection;
    }
}