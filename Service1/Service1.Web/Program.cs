using Microsoft.OpenApi.Models;
using Service1.Web;
using SharedMessages;

var builder = WebApplication.CreateBuilder(args);

// Register NServiceBusService and IMessageSession
// Register NServiceBusService and IMessageSession
builder.Services.AddSingleton<NServiceBusService>(provider =>
    new NServiceBusService("Service1"));
builder.Services.AddSingleton<IMessageSession>(provider =>
{
    var nServiceBusService = provider.GetRequiredService<NServiceBusService>();
    return nServiceBusService.MessageSession;
});
builder.Services.AddHostedService(provider => provider.GetRequiredService<NServiceBusService>());


builder.WebHost.UseUrls("http://localhost:5001");

//Logging
builder.Logging.AddSimpleConsole(console => {
    console.IncludeScopes = true;
    console.TimestampFormat = "HH:mm:ss.ffff ";
    //console.SingleLine = true;
});
builder.Logging.ClearProviders();
builder.Logging.AddDebug();


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service1 API", Version = "v1" });
});
builder.Services.AddTransient<SignupHandler>();

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service1 API v1"));
}

app.UseRouting();

app.MapControllers(); 
app.MapGet("/", () => "Service1 is running!");
app.Run();
