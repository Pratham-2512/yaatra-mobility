using backend.Models;

namespace backend.Repositories;

public interface ITripRepository
{
    Trip? GetById(string id);
    IEnumerable<Trip> GetActive();
    IEnumerable<Trip> GetAll();
    void Add(Trip trip);
    void Update(Trip trip);
}

public interface IDriverRepository
{
    Driver? GetById(string id);
    IEnumerable<Driver> GetOnline();
    IEnumerable<Driver> GetAll();
    void Update(Driver driver);
}
