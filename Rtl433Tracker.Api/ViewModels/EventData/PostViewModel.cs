using Rtl433Tracker.Data.Models;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rtl433Tracker.Api.ViewModels.EventData
{
    public class PostViewModel
    {
        [Required]
        public string DriverModel { get; set; }

        [Required]
        public string DriverId { get; set; }

        [Required]
        public DateTime? Time { get; set; }

        [Required]
        public IDictionary<string, string> Data { get; set; }

        public async Task<Event> ToDomainModel(IDeviceService deviceService)
        {
            return new Event
            {
                Device = await deviceService.GetOrCreateAsync(DriverModel, DriverId),
                Time = Time.Value,
                Data = Data.Select(eventData => new Data.Models.EventData
                {
                    Property = eventData.Key,
                    Value = eventData.Value
                }).ToList()
            };
        }
    }
}
