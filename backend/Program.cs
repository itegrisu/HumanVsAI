using AivsHuman.Api.Models;
using AivsHuman.Api.Services;
using AivsHuman.Api;
using AivsHuman.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000", 
                    "http://localhost:3001", 
                    "http://localhost:5173",
                    "https://human-vs-ai-swart.vercel.app",           // Ana Vercel domain
                    "https://human-vs-hi3o1jyrs-itegrisus-projects-2fd6c8d3.vercel.app",  // Deployment URL
                    "https://*.vercel.app"                            // Tüm Vercel subdomainleri
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // SignalR için gerekli
        });
});

// Add SignalR
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Register services
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Map SignalR hubs
app.MapHub<DuelHub>("/duelhub");

// Port configuration for deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
