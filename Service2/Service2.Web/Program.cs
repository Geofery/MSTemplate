using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;
using Domain.Repositories;
using Infrastructure.Repositories;
using MySqlConnector;
using SharedMessages;
using Application.Commands;
using Newtonsoft.Json;
using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Load user secrets for sensitive data
builder.Configuration.AddUserSecrets<Program>();

// Resolve the username and password for database connection
var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("{DbUsername}", dbUsername)
    .Replace("{DbPassword}", dbPassword);

// Register NServiceBus configuration
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("OrderService");

    // Configure transport
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../../Service1/Build/NServiceBusTransport");

    // Serialization, error queue, and audit queue
    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    // Configure the Newtonsoft.Json serializer
    var serialization = endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    serialization.Settings(new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented // Optional, for better readability
    });

    // Configure routing
    var routing = transport.Routing();
    routing.RouteToEndpoint(typeof(ValidateUser), "UserManagement");
    routing.RouteToEndpoint(typeof(SignupCommand), "UserManagement");
    routing.RouteToEndpoint(typeof(SaveOrder), "OrderService");
    routing.RouteToEndpoint(typeof(ProcessPayment), "PaymentService");
    //routing.RouteToEndpoint(typeof(CancelOrder), "OrderService");

    //TODO UPDATE DATABASE WITH NEW ATTRIBUTES --> Amount, Status, Reason to Order...

    // Configure Saga persistence
    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    var dialect = persistence.SqlDialect<SqlDialect.MySql>();
    persistence.ConnectionBuilder(() => new MySqlConnection(connectionString));
    persistence.TablePrefix("NSB_");
        persistence.DisableInstaller(); // Ensure installers are enabled for initial table creation

    endpointConfiguration.EnableInstallers(); // Enable installers to create tables


    return endpointConfiguration;
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(console =>
{
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
});
builder.Logging.AddDebug();

// Set application URL
builder.WebHost.UseUrls("http://localhost:5002");

// Register services in DI container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<SaveOrderHandler>();

// Add MVC controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});

// Build the application
var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API v1"));
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

// Map health-check and root routes
app.MapGet("/", () => Results.Ok("OrderService is running!"));
app.MapGet("/health-check", async (IOrderRepository orderRepository) =>
{
    try
    {
        var canConnect = await orderRepository.HealthCheckAsync();
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

app.Logger.LogInformation("OrderService is starting...");
app.Run();
