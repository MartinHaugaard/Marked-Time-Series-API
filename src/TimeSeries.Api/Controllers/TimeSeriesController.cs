using Microsoft.AspNetCore.Mvc;
using TimeSeries.Api.Dtos;
using TimeSeries.Api.Mapping;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeSeriesController : ControllerBase
{
    private readonly ITimeSeriesService _timeSeriesService;

    public TimeSeriesController(ITimeSeriesService timeSeriesService)
    {
        _timeSeriesService = timeSeriesService;
    }

    [HttpPost]
    public async Task<ActionResult<TimeSeriesResponse>> Create(CreateTimeSeriesRequest createRequest)
    {
        var model = createRequest.ToModel();
        var created = await _timeSeriesService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TimeSeriesResponse>> GetById(Guid id)
    {
        var record = await _timeSeriesService.GetAsync(id);
        return record.ToResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        await _timeSeriesService.DeleteAsync(id);
        return NoContent();
    }
}
