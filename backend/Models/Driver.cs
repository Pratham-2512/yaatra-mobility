namespace backend.Models;

public class Driver
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public double Rating { get; set; } = 4.9;
    public bool Online { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? ActiveTripId { get; set; }
    public decimal EarningsToday { get; set; }
    public int TripsToday { get; set; }
}
