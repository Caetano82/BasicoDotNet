using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using Bernhoeft.GRT.Core.Extensions;
using Bernhoeft.GRT.Teste.Api.Hubs;
using Bernhoeft.GRT.Teste.Api.Swashbuckle;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (Debugger.IsAttached)
    builder.Configuration.AddJsonFile("appsettings.Debugger.json", true);

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false; /* Header com Informações do Servidor. */
    options.Limits.MaxRequestLineSize = 32 * 1024; /* Tamanho máximo do URL (em bytes). */
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; /* Tamanho máximo da solicitação (em bytes). */
    options.Limits.MinResponseDataRate = null; /* Taxa mínima de dados de resposta (bytes/segundo). */
    options.Limits.MinRequestBodyDataRate = null; /* Taxa mínima de dados de solicitação (bytes/segundo). */
    if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
        options.ListenAnyIP(port);
});

// Adicionando SignalR primeiro (antes dos controllers para garantir que IHubContext esteja disponível)
builder.Services.AddSignalR();

// Adicionando os serviços no container.
builder.Services.AddMemoryCache()
                .AddControllers(options =>
                {
                    options.MaxModelBindingCollectionSize = int.MaxValue;
                    options.CacheProfiles.Add("DefaultCache", new CacheProfile
                    {
                        Duration = 3600,
                        Location = ResponseCacheLocation.Client
                    });
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

builder.Services.AddEndpointsApiExplorer();

// Configurando o versionamento.
builder.Services.AddApiVersioning(p =>
                {
                    p.DefaultApiVersion = new ApiVersion(1, 0);
                    p.ReportApiVersions = true;
                    p.AssumeDefaultVersionWhenUnspecified = true;
                })
                .AddApiExplorer(p =>
                {
                    p.GroupNameFormat = "'Teste API v'VVV";
                    p.SubstituteApiVersionInUrl = true;
                });

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.IgnoreObsoleteProperties();
    options.AddGRTSwaggerGenFilters();
    options.AddEnumsWithValuesFixFilters(option =>
    {
        option.ApplySchemaFilter = true;
        option.ApplyParameterFilter = true;
        option.ApplyDocumentFilter = true;
        option.IncludeDescriptions = true;
        option.IncludeXEnumRemarks = true;
        option.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
    });

    Directory.GetFiles(AppContext.BaseDirectory, "*.xml").ForEach(xmlFile =>
    {
        options.IncludeXmlComments(xmlFile, true);
    });
});

// Configurando o MediatR.
builder.Services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<GetAvisosRequest>());

// Adicionar Context de Conexão com Banco de Dados SqlServer GRT.
builder.Services.AddDbContext();

// Outros Serviços.
builder.Services.RegisterServicesFromAssemblyContaining<GetAvisosRequest>();

// Adicionando Fluent Validation.
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("pt-BR");
builder.Services.AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true)
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblyContaining<GetAvisosRequest>();
builder.Services.AddFluentValidationRulesToSwagger();

// Configure Some Options
builder.Services.Configure<FormOptions>(options => options.ValueCountLimit = int.MaxValue)
                .Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Configurando a Pipeline do HTTP Request.
var app = builder.Build();
app.UseForwardedHeaders(new()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || Debugger.IsAttached)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        apiVersionDescriptionProvider?.ApiVersionDescriptions?.Reverse().ForEach(description =>
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        });

        options.RoutePrefix = string.Empty;
    });
}

app.UseCors(options => options.WithOrigins()
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials()
                              .SetIsOriginAllowed(origin => true));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AvisosHub>("/hub/avisos");
await app.RunAsync();

public partial class Program
{ }