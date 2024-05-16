using Microsoft.Azure.Cosmos;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json.Converters;
using order_managment.Messaging;
using order_managment.Services;
using JwtManagerHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddConsole();

var configuration = builder.Configuration;

builder.Services.AddCustomJwtAuthentica();
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

builder.Services.AddControllers()
.AddNewtonsoftJson(jsonOptions =>
{
    jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    string redis = configuration.GetConnectionString("Redis")!;
    options.Configuration = redis;
    options.InstanceName = "SampleInstance";
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddSingleton<ITopicClient>(x =>
    new TopicClient(configuration.GetConnectionString("ServiceBus"), configuration.GetConnectionString("Topic"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
