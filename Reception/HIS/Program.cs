using HIS.Application.Extensions;
using HIS.infrastructure.Extensions;
using HIS.Presentation.HTTP.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddHisApplication();
builder.Services.AddInfrastructure();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddPresentationHttp();

builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile($"appsettings.json", true, true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5105", "http://localhost:5105")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "Client";
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowFrontend");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "client";
});

await app.RunAsync();