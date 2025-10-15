using Microsoft.Extensions.DependencyInjection;
using Reception.Application.Contracts;
using Reception.Application.Services;

namespace Reception.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddScoped<IPatientService, PatientService>();
        servicesCollection.AddScoped<IHl7ClientService, Hl7ClientService>();
        return servicesCollection;
    }
}