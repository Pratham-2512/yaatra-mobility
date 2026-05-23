using backend.DTOs;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController : ControllerBase
{
    private readonly IDriverRepository _drivers;

    public DriversController(IDriverRepository drivers) => _drivers = drivers;

    [HttpGet("online")]
    public ActionResult<IEnumerable<DriverDto>> GetOnline() =>
        Ok(_drivers.GetOnline().Select(d => new DriverDto(
            d.Id, d.Name, d.Vehicle, d.Plate, d.Rating, d.Online, d.Lat, d.Lng, d.ActiveTripId)));
}
