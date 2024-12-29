using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;
using Domain.Repositories;
using Infrastructure.Repositories;
using MySqlConnector;
using SharedMessages;
using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Resolve the username and password for database connection
var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("{DbUsername}", dbUsername)
    .Replace("{DbPassword}", dbPassword);

// Configure NServiceBus
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("PaymentService");

    // Configure transport
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../../Service1/Build/NServiceBusTransport");

    // Configure serialization, error queue, and audit queue
    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    return endpointConfiguration;
});

// Set application URL
builder.WebHost.UseUrls("http://localhost:5003");

// Register services in DI container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(console =>
{
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
});
builder.Logging.AddDebug();

// Register services in DI container
//builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>(); // Change to Scoped
builder.Services.AddScoped<ProcessPaymentHandler>();
builder.Services.AddScoped<PaymentFailedHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentService API", Version = "v1" });
});

// Build the application
var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService API v1"));
}

// Ensure database is created or updated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (dbContext.Database.EnsureCreated())
        {
            app.Logger.LogInformation("Database created successfully.");
        }
        else
        {
            app.Logger.LogInformation("Database already exists.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while ensuring the database is created or updated.");
    }
}

// Health-check and root routes
app.MapGet("/", () => Results.Ok("PaymentService is running!"));
app.MapGet("/health-check", async (IPaymentRepository paymentRepository) =>
{
    try
    {
        var healthCheck = await paymentRepository.HealthCheckAsync();
        return healthCheck ? Results.Ok("Repository is operational!") : Results.Problem("Repository health check failed.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Repository health check failed: {ex.Message}");
    }
});

// Configure routing and controllers
app.UseRouting();
app.MapControllers();

app.Logger.LogInformation("PaymentService is starting...");
app.Run();
