using AiAgent.Api.Extensions;
using AiAgent.Api.Domain.Common.Interfaces;
using Scalar.AspNetCore;
using AiAgent.Api.Auth;
using AiAgent.Api.Middleware;

namespace AiAgent.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.SetupConfiguration(builder.Configuration);
        builder.Services.SetupServices();
        builder.Services
            .AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, _ => { });
        builder.Services.AddAuthorization();

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

        var apiGroup = app.MapGroup("/").RequireAuthorization();

        // Map endpoints
        apiGroup.MapDomainEndpoints();
        app.MapHealthChecks($"/");

        // Seed initial API keys
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
            await seeder.SeedApiKeysAsync();
        }

        await app.RunAsync();
    }
}