using Microsoft.OpenApi.Models;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// NServiceBus Endpoint Configuration
var endpointConfiguration = new EndpointConfiguration("Service3");

// Configure the Serializer
endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

// Use the Learning Transport for local testing
var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("../Build/NServiceBusTransport");

// Start the NServiceBus endpoint
var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
builder.Services.AddSingleton(endpointInstance);

builder.WebHost.UseUrls("http://localhost:5003");

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service3 API", Version = "v1" });
});

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service3 API v1"));
}

app.MapGet("/", () => "Service3 is running!");

app.Run();
