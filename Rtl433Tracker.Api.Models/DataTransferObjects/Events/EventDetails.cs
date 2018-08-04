using System;

namespace Rtl433Tracker.Api.Models.DataTransferObjects.Events
{
    public class EventDetails
    {
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public Guid DeviceId { get; set; }

        public string DriverModel { get; set; }

        public string DriverId { get; set; }
    }
}
