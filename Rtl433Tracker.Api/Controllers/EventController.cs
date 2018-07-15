using Microsoft.AspNetCore.Mvc;
using Rtl433Tracker.Api.ViewModels.EventData;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rtl433Tracker.Api.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IDeviceService _deviceService;

        public EventController(IEventService eventService, IDeviceService deviceService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostViewModel eventVm)
        {
            var createdGuid = await _eventService.CreateAsync(await eventVm.ToDomainModel(_deviceService));
            return Ok(createdGuid);
        }
    }
}
