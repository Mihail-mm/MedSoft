using Microsoft.Extensions.DependencyInjection;

namespace HIS.Presentation.Fhir.Extensions;

public static class MvcBuilderFhirExtensions
{
    public static IMvcBuilder AddPresentationFhir(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(typeof(IAssemblyMarker).Assembly);
    }
}