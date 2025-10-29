using HIS.Presentation.Fhir.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HIS.Presentation.Fhir.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFhirServices(this IServiceCollection services)
    {
        services.AddScoped<IFhirPatientService, FhirPatientService>();
        return services;
    }
}