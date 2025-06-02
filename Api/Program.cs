using Api.Domain.AI.Extensions;
using Api.Domain.AI.Interfaces;
using Api.Domain.AI.Models;
using Api.Domain.AI.Services;
using Api.Domain.Configuration;
using Api.Domain.Database.Interfaces;
using Api.Domain.Database.Repository;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("AI:OpenAI"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("AI:Gemini"));
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.AddTransient<IOpenAiService, OpenAiService>();
builder.Services.AddTransient<IGeminiService, GeminiService>();

builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatGPT Challenge API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatGPT Challenge API v1");
    });
}

app.MapPost("/chat/azure", async (IOpenAiService openAiService, ChatRequest request) =>
{
    return await openAiService.Process(request.UserMessage);
});

app.MapPost("/chat/gemini", async (IGeminiService geminiService, ChatRequest request) =>
{
    return await geminiService.Process(request.UserMessage);
});

app.MapGet("/chat/history", async (IChatHistoryRepository repository) =>
{
    var history = await repository.GetChatHistoryAsync();
    return history.ToResponse();
});

app.Run();
