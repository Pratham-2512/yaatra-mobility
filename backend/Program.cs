using backend.Data;
using backend.Repositories;
using backend.Services;
using backend.Simulations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSingleton<ITripRepository, InMemoryTripRepository>();
builder.Services.AddSingleton<IDriverRepository>(sp =>
    new InMemoryDriverRepository(SeedData.CreateDrivers()));
builder.Services.AddSingleton<IPricingService, PricingService>();
builder.Services.AddSingleton<ITripService, TripService>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();
builder.Services.AddHostedService<FleetSimulation>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.MapControllers();

app.Run();
