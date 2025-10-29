using Doctor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Doctor.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDoctorApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPatientService, PatientService>();
        return services;
    }
}