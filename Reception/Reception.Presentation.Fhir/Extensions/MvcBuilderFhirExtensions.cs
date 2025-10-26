using Microsoft.Extensions.DependencyInjection;

namespace Reception.Presentation.Fhir.Extensions;

public static class MvcBuilderFhirExtensions
{
    public static IMvcBuilder AddPresentationFhir(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(typeof(IAssemblyMarker).Assembly);
    }
}