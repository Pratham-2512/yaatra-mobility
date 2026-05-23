using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripService _trips;

    public TripsController(ITripService trips) => _trips = trips;

    [HttpGet("active")]
    public ActionResult<IEnumerable<TripDto>> GetActive() =>
        Ok(_trips.GetActive());

    [HttpGet("{id}")]
    public ActionResult<TripDto> Get(string id)
    {
        var trip = _trips.Get(id);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpPost("create")]
    public ActionResult<TripDto> Create([FromBody] CreateTripRequest request) =>
        Ok(_trips.Create(request));

    [HttpPost("{id}/assign")]
    public ActionResult<TripDto> Assign(string id, [FromBody] AssignTripRequest request) =>
        Ok(_trips.Assign(id, request));

    [HttpPost("{id}/start")]
    public ActionResult<TripDto> Start(string id) =>
        Ok(_trips.Start(id));

    [HttpPost("complete")]
    public ActionResult<TripDto> Complete([FromBody] CompleteTripBody body) =>
        Ok(_trips.Complete(body.TripId, new CompleteTripRequest(body.Rating, body.Feedback)));

    [HttpPost("estimate-fare")]
    public ActionResult<FareEstimateDto> EstimateFare([FromBody] EstimateFareBody body) =>
        Ok(_trips.EstimateFare(body.Pickup, body.Dropoff, body.VehicleType));
}

public record CompleteTripBody(string TripId, int? Rating, string? Feedback);
public record EstimateFareBody(string Pickup, string Dropoff, string VehicleType);
