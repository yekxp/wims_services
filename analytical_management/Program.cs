using analytical_management.Model;
using Fleck;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddTransient<ISubscriptionClient>(x =>
   new SubscriptionClient(configuration.GetConnectionString("ServiceBus"), configuration.GetConnectionString("Topic"), configuration.GetConnectionString("Subscription"))
);

var app = builder.Build();
var _subscriptionClient = app.Services.GetService<ISubscriptionClient>()!;

var server = new WebSocketServer($"ws://0.0.0.0:8181");

var wsConnections = new List<IWebSocketConnection>();

server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        wsConnections.Add(ws);
    };

    ws.OnClose = () =>
    {
        wsConnections.Remove(ws);
        _subscriptionClient.UnregisterMessageHandlerAsync(TimeSpan.FromSeconds(3)).Wait();
    };


        // Register the message handler during service initialization (e.g., during application startup)
        _subscriptionClient.RegisterMessageHandler(async (message, token) =>
        {
            Order orderItems = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Body)}");

            foreach (var connection in wsConnections)
                await connection.Send($"{Encoding.UTF8.GetString(message.Body)}");

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }, new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            AutoComplete = false,
            MaxConcurrentCalls = 1
        });

 


    // Handle exceptions in the message handler
    Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        Console.WriteLine($"Message handler encountered an exception: {exceptionReceivedEventArgs.Exception}");
        return Task.CompletedTask;
    }


});

app.Run();