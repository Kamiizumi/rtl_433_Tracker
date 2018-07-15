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

        public EventController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostViewModel eventVm)
        {
            var createdGuid = await _eventService.CreateAsync(eventVm.ToDomainModel());
            return Ok(createdGuid);
        }
    }
}
