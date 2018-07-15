using Microsoft.EntityFrameworkCore;
using Rtl433Tracker.Data;
using Rtl433Tracker.Data.Models;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rtl433Tracker.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly Rtl433TrackerContext _rtl433TrackerContext;

        public DeviceService(Rtl433TrackerContext rtl433TrackerContext)
        {
            _rtl433TrackerContext = rtl433TrackerContext ?? throw new ArgumentNullException(nameof(rtl433TrackerContext));
        }

        public async Task<Device> GetOrCreateAsync(string driverModel, string driverId)
        {
            var device = await GetAsync(driverModel, driverId);

            if (device == null)
            {
                device = new Device
                {
                    DriverModel = driverModel,
                    DriverId = driverId
                };

                await CreateAsync(device);
            }

            return device;
        }

        public async Task<Guid> CreateAsync(Device device)
        {
            await _rtl433TrackerContext
                .Devices
                .AddAsync(device);
            await _rtl433TrackerContext.SaveChangesAsync();

            return device.Id;
        }

        public async Task<Device> GetAsync(string driverModel, string driverId)
        {
            return await _rtl433TrackerContext
                .Devices
                .SingleOrDefaultAsync(device =>
                    device.DriverModel == driverModel &&
                    device.DriverId == driverId);
        }
    }
}
