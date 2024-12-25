using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;
using Domain.Repositories;
using Infrastructure.Repositories;
using MySqlConnector;
using SharedMessages;
using Application.Commands;

var builder = WebApplication.CreateBuilder(args);

//DbContext to DI container
builder.Configuration.AddUserSecrets<Program>();

// Resolve the username and password from user secrets
var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];

// Replace placeholders in the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("{DbUsername}", dbUsername)
    .Replace("{DbPassword}", dbPassword);

// Register NServiceBusService and IMessageSession
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("OrderService");
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../../Service1/Build/NServiceBusTransport");

    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    // Routing configuration
    var routing = transport.Routing();
    routing.RouteToEndpoint(typeof(ValidateUser), "UserManagement");
    routing.RouteToEndpoint(typeof(SignupCommand), "UserManagement");
    routing.RouteToEndpoint(typeof(SaveOrder), "OrderService");

    //Saga Persistence
    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    var dialect = persistence.SqlDialect<SqlDialect.MySql>();
    persistence.ConnectionBuilder(() => new MySqlConnection(connectionString));
    persistence.TablePrefix("NSB_");


    return endpointConfiguration;
});

builder.Logging.AddSimpleConsole(console => {
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
});
builder.Logging.ClearProviders();
builder.Logging.AddDebug();

builder.WebHost.UseUrls("http://localhost:5002");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the DI container
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<SaveOrderHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});

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
    if (dbContext.Database.EnsureCreated())
    {
        Console.WriteLine("Database created successfully.");
    }
    else
    {
        Console.WriteLine("Database already exists.");
    }
}

app.MapGet("/", () => "OrderService is running!");
app.MapGet("/health-check", async (AppDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return canConnect ? Results.Ok("Database connection successful!") : Results.Problem("Cannot connect to the database.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});
app.UseRouting();
app.MapControllers();
Console.WriteLine("System should be running -_- ");

app.Run();

