using backend.Models;

namespace backend.Data;

public static class SeedData
{
    public static List<Driver> CreateDrivers() =>
    [
        new Driver { Id = "DRV-001", Name = "Raj Kumar", Vehicle = "Maruti Ertiga", Plate = "KA-02-AB-1234", Rating = 4.9, Online = true, Lat = 28.4595, Lng = 77.0266, EarningsToday = 4250, TripsToday = 18 },
        new Driver { Id = "DRV-002", Name = "Amit Singh", Vehicle = "Honda Activa", Plate = "DL-01-XY-5678", Rating = 4.8, Online = true, Lat = 28.4942, Lng = 77.0825, EarningsToday = 2100, TripsToday = 12 },
        new Driver { Id = "DRV-003", Name = "Suresh Yadav", Vehicle = "Bajaj RE", Plate = "HR-26-CD-9012", Rating = 4.7, Online = true, Lat = 28.472, Lng = 77.072, EarningsToday = 3800, TripsToday = 22 },
        new Driver { Id = "DRV-004", Name = "Priya Sharma", Vehicle = "Hyundai Creta", Plate = "DL-09-EF-3456", Rating = 4.95, Online = false, Lat = 28.448, Lng = 77.09, EarningsToday = 0, TripsToday = 0 },
        new Driver { Id = "DRV-005", Name = "Vikram Patel", Vehicle = "Toyota Innova", Plate = "GJ-05-GH-7890", Rating = 4.85, Online = true, Lat = 28.481, Lng = 77.068, EarningsToday = 5600, TripsToday = 15 },
    ];
}
