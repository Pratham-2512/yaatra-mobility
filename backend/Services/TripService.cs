using backend.DTOs;
using backend.Helpers;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

public interface ITripService
{
    TripDto Create(CreateTripRequest request);
    TripDto? Get(string id);
    IEnumerable<TripDto> GetActive();
    TripDto Assign(string tripId, AssignTripRequest request);
    TripDto Start(string tripId);
    TripDto Complete(string tripId, CompleteTripRequest request);
    FareEstimateDto EstimateFare(string pickup, string dropoff, string vehicleType);
}

public class TripService : ITripService
{
    private readonly ITripRepository _trips;
    private readonly IDriverRepository _drivers;
    private readonly IPricingService _pricing;

    public TripService(ITripRepository trips, IDriverRepository drivers, IPricingService pricing)
    {
        _trips = trips;
        _drivers = drivers;
        _pricing = pricing;
    }

    public FareEstimateDto EstimateFare(string pickup, string dropoff, string vehicleType)
    {
        var (distance, duration) = ResolveDistance(pickup, dropoff);
        var traffic = duration > 20 ? 1.15 : 1.0;
        var (total, baseFare, dist, surge) = _pricing.Calculate(vehicleType, distance, duration, traffic);
        return new FareEstimateDto(total, baseFare, dist, surge, distance, duration);
    }

    public TripDto Create(CreateTripRequest request)
    {
        var (distance, duration) = ResolveDistance(request.Pickup, request.Dropoff);
        var traffic = duration > 20 ? 1.2 : 1.0;
        var (total, baseFare, _, surge) = _pricing.Calculate(request.VehicleType, distance, duration, traffic);

        var trip = new Trip
        {
            Id = $"T-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            Pickup = request.Pickup,
            Dropoff = request.Dropoff,
            VehicleType = request.VehicleType,
            Status = TripStatus.Searching,
            DistanceKm = distance,
            DurationMin = duration,
            TotalFare = total,
            BaseFare = baseFare,
            SurgeFare = surge,
            EtaMin = Math.Max(3, duration / 3),
            Progress = 0,
        };

        _trips.Add(trip);
        return ToDto(trip);
    }

    public TripDto? Get(string id)
    {
        var trip = _trips.GetById(id);
        return trip is null ? null : ToDto(trip);
    }

    public IEnumerable<TripDto> GetActive() =>
        _trips.GetActive().Select(ToDto).OrderByDescending(t => t.CreatedAt);

    public TripDto Assign(string tripId, AssignTripRequest request)
    {
        var trip = _trips.GetById(tripId) ?? throw new KeyNotFoundException("Trip not found");
        var driver = _drivers.GetById(request.DriverId);
        trip.DriverId = request.DriverId;
        trip.DriverName = request.DriverName ?? driver?.Name ?? "Fleet Partner";
        trip.Status = TripStatus.Arriving;
        trip.EtaMin = 4;
        if (driver is not null)
        {
            trip.DriverLat = driver.Lat;
            trip.DriverLng = driver.Lng;
            driver.ActiveTripId = tripId;
            _drivers.Update(driver);
        }
        _trips.Update(trip);
        return ToDto(trip);
    }

    public TripDto Start(string tripId)
    {
        var trip = _trips.GetById(tripId) ?? throw new KeyNotFoundException("Trip not found");
        trip.Status = TripStatus.InProgress;
        trip.Progress = 0;
        _trips.Update(trip);
        return ToDto(trip);
    }

    public TripDto Complete(string tripId, CompleteTripRequest request)
    {
        var trip = _trips.GetById(tripId) ?? throw new KeyNotFoundException("Trip not found");
        trip.Status = TripStatus.Completed;
        trip.Progress = 100;
        trip.Rating = request.Rating;
        trip.Feedback = request.Feedback;

        if (!string.IsNullOrEmpty(trip.DriverId))
        {
            var driver = _drivers.GetById(trip.DriverId);
            if (driver is not null)
            {
                driver.ActiveTripId = null;
                driver.EarningsToday += trip.TotalFare;
                driver.TripsToday++;
                _drivers.Update(driver);
            }
        }

        _trips.Update(trip);
        return ToDto(trip);
    }

    private static (double Distance, int Duration) ResolveDistance(string pickup, string dropoff)
    {
        var p = NcrGeocoding.Resolve(pickup);
        var d = NcrGeocoding.Resolve(dropoff);
        var km = NcrGeocoding.DistanceKm(p, d);
        if (km < 0.5) km = 2.5;
        var duration = Math.Max(3, (int)Math.Ceiling(km / 22.0 * 60));
        return (Math.Round(km, 1), duration);
    }

    internal static TripDto ToDto(Trip t) => new(
        t.Id,
        t.Pickup,
        t.Dropoff,
        t.VehicleType,
        t.Status.ToString(),
        t.DriverId,
        t.DriverName,
        t.DistanceKm,
        t.DurationMin,
        t.TotalFare,
        t.BaseFare,
        t.SurgeFare,
        t.Progress,
        t.EtaMin,
        t.DriverLat,
        t.DriverLng,
        t.CreatedAt
    );
}
