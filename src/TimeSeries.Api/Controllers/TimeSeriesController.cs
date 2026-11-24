using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeSeries.Api.Dtos;
using TimeSeries.Api.Mapping;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Api.Controllers
{
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
        public ActionResult<TimeSeriesResponse> Create(CreateTimeSeriesRequest createRequest)
        {
            var model = createRequest.ToModel();
            var created = _timeSeriesService.Create(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse()); ;
        }

        [HttpGet("{id:guid}")]
        public ActionResult<TimeSeriesResponse> GetById(Guid id)
        {
            var record = _timeSeriesService.Get(id);
            return record.ToResponse();
        }
    }
}
