using Microsoft.OpenApi.Models;
using SharedMessages;

var builder = WebApplication.CreateBuilder(args);

// Register NServiceBusService and IMessageSession
builder.Services.AddSingleton<NServiceBusService>(provider =>
    new NServiceBusService("Service2"));
builder.Services.AddSingleton<IMessageSession>(provider =>
{
    var nServiceBusService = provider.GetRequiredService<NServiceBusService>();
    return nServiceBusService.MessageSession;
});
builder.Services.AddHostedService(provider => provider.GetRequiredService<NServiceBusService>());

builder.WebHost.UseUrls("http://localhost:5002");


// Add services to the container
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
