using Microsoft.OpenApi.Models;
using Web;
using SharedMessages;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register NServiceBusService and IMessageSession
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("UserManagement");
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../Build/NServiceBusTransport");

    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    return endpointConfiguration;
});

builder.WebHost.UseUrls("http://localhost:5001");

//Logging
builder.Logging.AddSimpleConsole(console => {
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
});
builder.Logging.ClearProviders();
builder.Logging.AddDebug();

//DbContext to DI container
builder.Configuration.AddUserSecrets<Program>();

// Resolve the username and password from user secrets
var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];

// Replace placeholders in the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("{DbUsername}", dbUsername)
    .Replace("{DbPassword}", dbPassword);
// Register DbContext with MySQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Add services to the container
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<SignupHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagement API", Version = "v1" });
});

var serviceProvider = builder.Services.BuildServiceProvider();
var signupHandler = serviceProvider.GetService<SignupHandler>();
var userRepo = serviceProvider.GetService<IUserRepository>();
if (signupHandler == null || userRepo == null)
{
    Console.WriteLine($"SignupHandler or UserRepository is not resolved! SignupHandler: {signupHandler} Repo: {userRepo}?");
}

// Other service registrations
var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement API v1"));
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

app.MapGet("/", () => "UserManagement is running!");
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
app.Run();
