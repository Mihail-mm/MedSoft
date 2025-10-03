using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reception.Application.Extensions;
using Reception.Infrastructure.Extensions;
using Reception.Presentation.Http.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddPresentationHttp();

builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunAsync();