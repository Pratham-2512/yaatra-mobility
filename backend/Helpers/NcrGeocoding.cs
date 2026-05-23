namespace backend.Helpers;

public record GeoPoint(double Lng, double Lat, string Normalized, bool Matched);

public static class NcrGeocoding
{
    private static readonly (string[] Keywords, double Lng, double Lat, string Address)[] Zones =
    [
        (["cyber hub", "cyberhub", "dlf cyber"], 77.0825, 28.4942, "Cyber Hub, Gurgaon"),
        (["huda city", "huda metro"], 77.0728, 28.4591, "Huda City Centre, Gurgaon"),
        (["mg road", "mg road gurgaon"], 77.068, 28.481, "MG Road, Gurgaon"),
        (["iffco", "iffco chowk"], 77.072, 28.472, "IFFCO Chowk, Gurgaon"),
        (["sector 22", "sec 22"], 77.048, 28.508, "Sector 22, Gurgaon"),
        (["sector 43", "sec 43"], 77.09, 28.448, "Sector 43, Gurgaon"),
        (["connaught", "cp delhi"], 77.209, 28.6315, "Connaught Place, Delhi"),
        (["dwarka"], 77.0402, 28.5921, "Dwarka, Delhi"),
        (["noida", "sector 18 noida"], 77.324, 28.5706, "Sector 18, Noida"),
    ];

    public static GeoPoint Resolve(string input)
    {
        var raw = (input ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return new GeoPoint(77.1025, 28.4595, "Gurgaon, Haryana", false);

        var lower = raw.ToLowerInvariant();
        foreach (var (keywords, lng, lat, address) in Zones)
        {
            if (keywords.Any(k => lower.Contains(k)))
                return new GeoPoint(lng, lat, address, true);
        }

        var sectorMatch = System.Text.RegularExpressions.Regex.Match(lower, @"sector\s*(\d+)|sec\.?\s*(\d+)");
        if (sectorMatch.Success)
        {
            var num = int.Parse(sectorMatch.Groups[1].Success ? sectorMatch.Groups[1].Value : sectorMatch.Groups[2].Value);
            var offset = (num % 20) * 0.002;
            return new GeoPoint(77.04 + offset, 28.45 + offset * 0.8, $"Sector {num}, Gurgaon", true);
        }

        if (lower.Contains("gurgaon") || lower.Contains("gurugram"))
            return new GeoPoint(77.0689, 28.4595, raw, true);

        return new GeoPoint(
            77.08 + (raw.Length % 10) * 0.003,
            28.46 + (raw.Length % 8) * 0.002,
            $"{raw}, Gurgaon, Haryana",
            false);
    }

    public static double DistanceKm(GeoPoint a, GeoPoint b)
    {
        const double R = 6371;
        var dLat = (b.Lat - a.Lat) * Math.PI / 180;
        var dLng = (b.Lng - a.Lng) * Math.PI / 180;
        var lat1 = a.Lat * Math.PI / 180;
        var lat2 = b.Lat * Math.PI / 180;
        var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
    }
}
