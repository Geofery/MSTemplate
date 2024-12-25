using Microsoft.OpenApi.Models;
using Web;
using SharedMessages;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using MySqlConnector;

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

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "OrderService is running!");
app.Run();
