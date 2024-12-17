using NServiceBus;
using NServiceBus.Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// NServiceBus Endpoint Configuration
var endpointConfiguration = new EndpointConfiguration("Service1");

// Configure the Serializer
endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

// Use the Learning Transport for local testing
var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("../Build/NServiceBusTransport");

// Start the NServiceBus endpoint
var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
builder.Services.AddSingleton(endpointInstance);

builder.WebHost.UseUrls("http://localhost:5001");


var app = builder.Build();

app.MapGet("/", () => "Service1 is running!");

app.Run();
