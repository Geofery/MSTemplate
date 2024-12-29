using Microsoft.OpenApi.Models;
using Web;
using SharedMessages;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;
using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Register NServiceBus with configurations
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("UserManagement");

    // Configure transport
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../Build/NServiceBusTransport");

    // Set serialization, error queue, and audit queue
    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    return endpointConfiguration;
});

// Configure the application URL
builder.WebHost.UseUrls("http://localhost:5001");

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(console =>
{
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
});
builder.Logging.AddDebug();

// Load sensitive data from user secrets
builder.Configuration.AddUserSecrets<Program>();

// Resolve the username and password from user secrets for database connection
var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("{DbUsername}", dbUsername)
    .Replace("{DbPassword}", dbPassword);

// Register DbContext with MySQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<SignupHandler>();

// Register controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagement API", Version = "v1" });
});

// Build the application
var app = builder.Build();

// Configure Swagger for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement API v1"));
}

// Ensure the database is created or updated
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

// Map health-check and default routes
app.MapGet("/", () => Results.Ok("UserManagement is running!"));
app.MapGet("/health-check", async (IUserRepository userRepository) =>
{
    try
    {
        var canConnect = await userRepository.HealthCheckAsync();
        return canConnect ? Results.Ok("Database connection successful!") : Results.Problem("Cannot connect to the database.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// Configure routing and controllers
app.UseRouting();
app.MapControllers();
app.Run();
