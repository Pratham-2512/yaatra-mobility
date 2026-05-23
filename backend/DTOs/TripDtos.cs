namespace backend.DTOs;

public record CreateTripRequest(string Pickup, string Dropoff, string VehicleType, string? RiderId);

public record AssignTripRequest(string DriverId, string? DriverName);

public record CompleteTripRequest(int? Rating, string? Feedback);

public record TripDto(
    string Id,
    string Pickup,
    string Dropoff,
    string VehicleType,
    string Status,
    string? DriverId,
    string? DriverName,
    double DistanceKm,
    int DurationMin,
    decimal TotalFare,
    decimal BaseFare,
    decimal SurgeFare,
    double Progress,
    int EtaMin,
    double? DriverLat,
    double? DriverLng,
    DateTime CreatedAt
);

public record FareEstimateDto(
    decimal TotalFare,
    decimal Base,
    decimal DistanceCharge,
    decimal Surge,
    double DistanceKm,
    int DurationMin
);

public record LiveMetricsDto(
    int ActiveTrips,
    int OnlineDrivers,
    decimal RevenueToday,
    double AvgRating,
    int DelayedTrips,
    int FleetUtilization,
    double CityVelocityKmh,
    List<ActiveTripSummaryDto> ActiveTripList,
    List<AnomalyDto> Anomalies,
    List<PredictionDto> Predictions
);

public record ActiveTripSummaryDto(string Id, string Route, string Status, bool Delayed);

public record AnomalyDto(string Severity, string DriverId, double ExpectedDistance, double ActualDistance, double Variance);

public record PredictionDto(string Message);

public record DriverDto(
    string Id,
    string Name,
    string Vehicle,
    string Plate,
    double Rating,
    bool Online,
    double Lat,
    double Lng,
    string? ActiveTripId
);
