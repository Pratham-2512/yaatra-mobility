using System.Collections.Concurrent;
using backend.Models;

namespace backend.Repositories;

public class InMemoryTripRepository : ITripRepository
{
    private readonly ConcurrentDictionary<string, Trip> _trips = new();

    public Trip? GetById(string id) => _trips.TryGetValue(id, out var t) ? t : null;

    public IEnumerable<Trip> GetActive() =>
        _trips.Values.Where(t => t.Status is not TripStatus.Completed and not TripStatus.Cancelled);

    public IEnumerable<Trip> GetAll() => _trips.Values;

    public void Add(Trip trip) => _trips[trip.Id] = trip;

    public void Update(Trip trip)
    {
        trip.UpdatedAt = DateTime.UtcNow;
        _trips[trip.Id] = trip;
    }
}

public class InMemoryDriverRepository : IDriverRepository
{
    private readonly ConcurrentDictionary<string, Driver> _drivers = new();

    public InMemoryDriverRepository(IEnumerable<Driver> seed) =>
        seed.ToList().ForEach(d => _drivers[d.Id] = d);

    public Driver? GetById(string id) => _drivers.TryGetValue(id, out var d) ? d : null;

    public IEnumerable<Driver> GetOnline() => _drivers.Values.Where(d => d.Online);

    public IEnumerable<Driver> GetAll() => _drivers.Values;

    public void Update(Driver driver) => _drivers[driver.Id] = driver;
}
