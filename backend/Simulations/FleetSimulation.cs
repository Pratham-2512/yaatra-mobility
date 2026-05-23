using backend.Models;
using backend.Repositories;

namespace backend.Simulations;

public class FleetSimulation : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<FleetSimulation> _logger;

    public FleetSimulation(IServiceProvider services, ILogger<FleetSimulation> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var trips = scope.ServiceProvider.GetRequiredService<ITripRepository>();
                var drivers = scope.ServiceProvider.GetRequiredService<IDriverRepository>();

                foreach (var trip in trips.GetActive())
                {
                    if (trip.Status == TripStatus.InProgress && trip.Progress < 100)
                    {
                        trip.Progress = Math.Min(100, trip.Progress + 3 + Random.Shared.NextDouble() * 5);
                        trip.EtaMin = Math.Max(1, (int)((100 - trip.Progress) / 12));
                        if (trip.DriverLat.HasValue && trip.DriverLng.HasValue)
                        {
                            trip.DriverLat += 0.0003 * Random.Shared.NextDouble();
                            trip.DriverLng += 0.0004 * Random.Shared.NextDouble();
                        }
                        trips.Update(trip);
                    }
                    else if (trip.Status == TripStatus.Arriving && trip.Progress < 18)
                    {
                        trip.Progress = Math.Min(18, trip.Progress + 2);
                        trip.EtaMin = Math.Max(1, 5 - (int)(trip.Progress / 4));
                        if (trip.DriverLat.HasValue)
                        {
                            trip.DriverLat += 0.0002;
                            trip.DriverLng = (trip.DriverLng ?? 77.07) + 0.00025;
                        }
                        trips.Update(trip);
                    }
                }

                foreach (var driver in drivers.GetOnline())
                {
                    if (string.IsNullOrEmpty(driver.ActiveTripId))
                    {
                        driver.Lat += (Random.Shared.NextDouble() - 0.5) * 0.0004;
                        driver.Lng += (Random.Shared.NextDouble() - 0.5) * 0.0004;
                        drivers.Update(driver);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fleet simulation tick failed");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }
}
