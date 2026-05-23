using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metrics;

    public MetricsController(IMetricsService metrics) => _metrics = metrics;

    [HttpGet("live")]
    public ActionResult GetLive() => Ok(_metrics.GetLive());
}

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IMetricsService _metrics;

    public AdminController(IMetricsService metrics) => _metrics = metrics;

    [HttpGet("metrics")]
    public ActionResult GetMetrics() => Ok(_metrics.GetLive());

    [HttpGet("anomalies")]
    public ActionResult GetAnomalies() => Ok(_metrics.GetLive().Anomalies);

    [HttpGet("ml-predictions")]
    public ActionResult GetPredictions() => Ok(_metrics.GetLive().Predictions);
}
