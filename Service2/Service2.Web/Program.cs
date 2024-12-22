using Microsoft.OpenApi.Models;
using Web;
using SharedMessages;
using Microsoft.EntityFrameworkCore;
using Web.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register NServiceBusService and IMessageSession
builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("Service2");
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    transport.StorageDirectory("../../Service1/Build/NServiceBusTransport");

    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.AuditProcessedMessagesTo("audit");

    return endpointConfiguration;
});

builder.WebHost.UseUrls("http://localhost:5002");


// Add services to the container
builder.Services.AddTransient<MessageHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service2 API", Version = "v1" });
});

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service2 API v1"));
}

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "Service2 is running!");
app.Run();
