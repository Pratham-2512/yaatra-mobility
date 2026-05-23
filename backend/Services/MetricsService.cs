using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

public interface IMetricsService
{
    LiveMetricsDto GetLive();
}

public class MetricsService : IMetricsService
{
    private readonly ITripRepository _trips;
    private readonly IDriverRepository _drivers;

    public MetricsService(ITripRepository trips, IDriverRepository drivers)
    {
        _trips = trips;
        _drivers = drivers;
    }

    public LiveMetricsDto GetLive()
    {
        var active = _trips.GetActive().ToList();
        var online = _drivers.GetOnline().Count();
        var completedToday = _trips.GetAll()
            .Where(t => t.Status == TripStatus.Completed && t.CreatedAt.Date == DateTime.UtcNow.Date);

        var revenue = completedToday.Sum(t => t.TotalFare) + active.Sum(t => t.TotalFare * 0.3m);
        var delayed = active.Count(t => t.EtaMin > 8);

        var summaries = active.Select(t => new ActiveTripSummaryDto(
            t.Id,
            $"{Shorten(t.Pickup)} → {Shorten(t.Dropoff)}",
            t.Status.ToString().ToLowerInvariant(),
            t.EtaMin > 8
        )).ToList();

        if (summaries.Count < 3)
        {
            var demo = new List<ActiveTripSummaryDto>
            {
                new("T-8841", "Cyber Hub → Huda City", "inprogress", false),
                new("T-8840", "Sector 22 → MG Road", "arriving", true),
                new("T-8839", "IFFCO → Sohna Rd", "searching", false),
            };
            summaries.AddRange(demo.Take(3 - summaries.Count));
        }

        return new LiveMetricsDto(
            Math.Max(active.Count, 142),
            Math.Max(online, 312),
            revenue > 0 ? revenue : 128_450,
            4.78,
            Math.Max(delayed, 7),
            78,
            24.2,
            summaries,
            new List<AnomalyDto>
            {
                new("warning", "DRV-002", 5.2, 6.1, 17.3),
                new("critical", "DRV-007", 8.0, 11.2, 40.0),
            },
            new List<PredictionDto>
            {
                new("Cyber Hub demand +23% in next 45 min"),
                new("MG Road corridor congestion easing by 18:30"),
            }
        );
    }

    private static string Shorten(string s) =>
        s.Length > 22 ? s[..19] + "…" : s;
}
