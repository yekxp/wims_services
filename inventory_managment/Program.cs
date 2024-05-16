using Azure.Messaging.ServiceBus;
using inventory_managment.Messaging;
using inventory_managment.Model.message;
using inventory_managment.Services;
using JwtManagerHandler;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddConsole();

var configuration = builder.Configuration;

builder.Services.AddSingleton((privider) =>
{
    var endpointUri = configuration["CosmosDbSettings:EndpointUri"];
    var primaryKey = configuration["CosmosDbSettings:PrimaryKey"];
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = databaseName
    };

    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    var cosmosClient = new CosmosClient(endpointUri, primaryKey, cosmosClientOptions);
    cosmosClient.ClientOptions.ConnectionMode = ConnectionMode.Direct;

    return cosmosClient;
});

builder.Services.AddCustomJwtAuthentica();

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson(jsonOptions =>
{
    jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = configuration["ServiceBus:Uri"], // RabbitMQ server address
        Port = int.Parse(configuration["ServiceBus:Port"]!), // RabbitMQ port
        UserName = configuration["ServiceBus:Username"], // RabbitMQ username
        Password = configuration["ServiceBus:Password"] // RabbitMQ password
    };

    return factory;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddHostedService<MessageReceiver>();

builder.Services.AddSingleton<ISubscriptionClient>(x =>
   new SubscriptionClient(configuration.GetConnectionString("ServiceBus"), configuration.GetConnectionString("Topic"), configuration.GetConnectionString("Subscription"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();