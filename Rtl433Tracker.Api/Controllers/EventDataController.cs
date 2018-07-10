using Microsoft.AspNetCore.Mvc;
using Rtl433Tracker.Api.ViewModels.EventData;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rtl433Tracker.Api.Controllers
{
    [Route("api/[controller]")]
    public class EventDataController : Controller
    {
        private readonly IEventDataService _eventDataService;

        public EventDataController(IEventDataService eventDataService)
        {
            _eventDataService = eventDataService ?? throw new ArgumentNullException(nameof(eventDataService));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostViewModel eventData)
        {
            var createdGuid = await _eventDataService.CreateAsync(eventData.ToDomainModel());
            return Ok(createdGuid);
        }
    }
}
