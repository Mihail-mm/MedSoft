using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reception.Application.Extensions;
using Reception.Infrastructure.Extensions;
using Reception.Presentation.Fhir.Extensions;
using Reception.Presentation.Http.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var certPath = Path.Combine(AppContext.BaseDirectory, "certs", "localhost.pfx");
var certPassword = "123456!";

if (!File.Exists(certPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(certPath)!);

    using var rsa = RSA.Create(2048);
    var req = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    req.CertificateExtensions.Add(
        new X509BasicConstraintsExtension(false, false, 0, false));
    req.CertificateExtensions.Add(
        new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
    req.CertificateExtensions.Add(
        new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

    var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3));
    var export = cert.Export(X509ContentType.Pfx, certPassword);
    File.WriteAllBytes(certPath, export);
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7066, listenOptions => { listenOptions.UseHttps(certPath, certPassword); });
});

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddPresentationHttp()
    .AddPresentationFhir();

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => 
    {
        return type.FullName.Replace(".", "_");
    });
}).AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile($"appsettings.json", true, true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7066", "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "Client"; });

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

app.UseSpa(spa => { spa.Options.SourcePath = "client"; });

await app.RunAsync();