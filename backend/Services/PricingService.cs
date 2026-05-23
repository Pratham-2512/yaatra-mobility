namespace backend.Services;

public interface IPricingService
{
    (decimal Total, decimal Base, decimal Distance, decimal Surge) Calculate(string vehicleType, double distanceKm, int durationMin, double trafficMultiplier = 1.0);
}

public class PricingService : IPricingService
{
    private static readonly Dictionary<string, (decimal Base, decimal PerKm)> Rates = new()
    {
        ["bike"] = (25, 8),
        ["auto"] = (35, 12),
        ["sedan"] = (50, 18),
        ["suv"] = (80, 28),
    };

    public (decimal Total, decimal Base, decimal Distance, decimal Surge) Calculate(
        string vehicleType,
        double distanceKm,
        int durationMin,
        double trafficMultiplier = 1.0)
    {
        var key = vehicleType.ToLowerInvariant();
        if (!Rates.TryGetValue(key, out var rate))
            rate = Rates["sedan"];

        if (distanceKm < 0.5) distanceKm = 2.5;

        var baseFare = rate.Base;
        var distanceCharge = (decimal)Math.Round(distanceKm * (double)rate.PerKm * trafficMultiplier);
        var surge = durationMin > 25 ? Math.Round(baseFare * 0.15m) : 0m;
        if (trafficMultiplier > 1.2)
            surge += Math.Round(baseFare * 0.1m);

        var total = baseFare + distanceCharge + surge;
        return (total, baseFare, distanceCharge, surge);
    }
}
