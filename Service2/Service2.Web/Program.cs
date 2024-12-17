using Microsoft.OpenApi.Models;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// NServiceBus Endpoint Configuration
var endpointConfiguration = new EndpointConfiguration("Service2");

// Configure the Serializer
endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

// Use the Learning Transport for local testing
var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("../Build/NServiceBusTransport");

// Start the NServiceBus endpoint
var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
builder.Services.AddSingleton(endpointInstance);

builder.WebHost.UseUrls("http://localhost:5002");

// Add services to the container
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

app.MapGet("/", () => "Service2 is running!");

app.Run();
