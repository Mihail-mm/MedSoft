using HIS.Application.Contracts;
using HIS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HIS.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHisApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPatientService, PatientService>();
        services.AddHostedService<Hl7BackgroundService>();
        return services;
    }
}