namespace backend.Models;

public class Trip
{
    public string Id { get; set; } = string.Empty;
    public string Pickup { get; set; } = string.Empty;
    public string Dropoff { get; set; } = string.Empty;
    public string VehicleType { get; set; } = "sedan";
    public TripStatus Status { get; set; } = TripStatus.Created;
    public string? DriverId { get; set; }
    public string? DriverName { get; set; }
    public double DistanceKm { get; set; }
    public int DurationMin { get; set; }
    public decimal TotalFare { get; set; }
    public decimal BaseFare { get; set; }
    public decimal SurgeFare { get; set; }
    public double Progress { get; set; }
    public int EtaMin { get; set; } = 5;
    public double? DriverLat { get; set; }
    public double? DriverLng { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? Rating { get; set; }
    public string? Feedback { get; set; }
}
