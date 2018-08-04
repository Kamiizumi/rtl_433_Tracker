using Microsoft.AspNetCore.Mvc;
using Rtl433Tracker.Api.Models.DataTransferObjects.Events;
using Rtl433Tracker.Api.ViewModels.EventData;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var eventDetails = _eventService
                .GetEvents()
                .OrderByDescending(singleEvent => singleEvent.Time)
                .Take(100)
                .Select(singleEvent => new EventDetails
                {
                    Id = singleEvent.Id,
                    Time = singleEvent.Time,
                    DeviceId = singleEvent.Device.Id,
                    DriverModel = singleEvent.Device.DriverModel,
                    DriverId = singleEvent.Device.DriverId
                });

            return new OkObjectResult(eventDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostViewModel eventVm)
        {
            var createdGuid = await _eventService.CreateAsync(await eventVm.ToDomainModel(_deviceService));
            return Ok(createdGuid);
        }

        [HttpPost]
        [Route(nameof(Rtl433Json))]
        public async Task<IActionResult> Rtl433Json([FromBody]IDictionary<string, string> rawData)
        {
            rawData.TryGetValue("model", out string driverModel);
            rawData.TryGetValue("id", out string driverId);
            rawData.TryGetValue("time", out string time);

            var vm = new PostViewModel
            {
                DriverModel = driverModel,
                DriverId = driverId,
                Time = Convert.ToDateTime(time),
                Data = rawData
                    .Where(a => a.Key != "model" && a.Key != "id" && a.Key != "time")
                    .ToDictionary(a => a.Key, a => a.Value)
            };

            var createdGuid = await _eventService.CreateAsync(await vm.ToDomainModel(_deviceService));
            return Ok(createdGuid);
        }
    }
}
