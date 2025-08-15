using System.Runtime.CompilerServices;
using AiAgent.Api.Extensions;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using AiAgent.Api.Auth;
using AiAgent.Api.Domain.Common.Enums;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Middleware;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Instructions; // Added

[assembly: InternalsVisibleTo("AiAgent.Api.IntegrationTests")]

namespace AiAgent.Api;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.SetupServices(builder.Configuration);
        builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme, options => { });
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            var showScalar = builder.Configuration.GetValue<bool>("ShowScalar");
            if (showScalar)
            {
                app.MapScalarApiReference();
            }
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/").RequireAuthorization();

        app.MapHealthChecks("/");

        // Map domain-specific endpoints
        app.MapInstructionsEndpoints(); // Moved to Instructions domain

        // Knowledge endpoints are now handled by KnowledgeController
        // app.MapPost("/knowledge/upload", ...)
        // app.MapGet("/knowledge/get/{key}/{module}", ...)
        // app.MapGet("/knowledge/all/{module}", ...)

        app.MapControllers();

        // Seed initial API keys
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
            await seeder.SeedAllDataAsync();
        }

        app.Run();
    }
}
