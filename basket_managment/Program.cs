using basket_managment.Services;
using JwtManagerHandler;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddCustomJwtAuthentica();

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    string redis = configuration.GetConnectionString("Redis")!;
    options.Configuration = redis;
    options.InstanceName = "SampleInstance";
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
