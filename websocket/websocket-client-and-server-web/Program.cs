using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container.
builder.Services.AddControllers(); // Updated to use Controllers in .NET 8
builder.Services.AddRazorPages();  // Razor Pages added as part of MVC

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.UseWebSockets();
app.Use(async (http, next) =>
{
    if (http.WebSockets.IsWebSocketRequest && http.Request.Path == "/ws")
    {
        var websocket = await http.WebSockets.AcceptWebSocketAsync();
        Log.Information("WebSocket connection established."); // Log when a connection is established

        await Task.Run(async () =>
        {
            while (websocket.State == System.Net.WebSockets.WebSocketState.Open)
            {
                byte[] buffer = new byte[1024];
                var result = await websocket.ReceiveAsync(buffer, CancellationToken.None);
                Log.Debug("Received message: {Message}", Encoding.UTF8.GetString(buffer, 0, result.Count)); // Log received messages

                await websocket.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
                Log.Information("Sent message back to client."); // Log when a message is sent back
            }
        });
    }
    else
    {
        await next();
    }
});

app.MapControllers(); // Mapping controllers in .NET 8

app.Run();
