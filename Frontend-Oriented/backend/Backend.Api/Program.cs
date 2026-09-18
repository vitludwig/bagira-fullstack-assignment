using System.Text.Json.Serialization;
using Backend.Api.DTOs;
using Backend.Api.Mappings;
using Backend.Api.Services;
using Backend.Api.Services.Interfaces;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();

var mappingConfig = TypeAdapterConfig.GlobalSettings;
mappingConfig.Scan(typeof(ScenarioMappingConfig).Assembly);
builder.Services.AddSingleton(mappingConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Scenario Builder API",
        Version = "v1",
        Description = "API for managing training scenarios and entities"
    });
});

var frontendOrigin = builder.Configuration["FrontendOrigin"] ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IScenarioRepository, ScenarioRepository>();
builder.Services.AddScoped<IEntityRepository, EntityRepository>();
builder.Services.AddScoped<IScenarioService, ScenarioService>();
builder.Services.AddScoped<IEntityService, EntityService>();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
            return;
        }

        const string errorMessage = "An unexpected error occurred.";
        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");

        logger.LogError(
            exception,
            "{ErrorMessage} Request: {Method} {Path}",
            errorMessage,
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ErrorResponse { Message = errorMessage });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Configuration.GetValue("UseHttpsRedirection", true))
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
