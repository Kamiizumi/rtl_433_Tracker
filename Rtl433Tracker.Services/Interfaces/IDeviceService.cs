using System;
using System.Threading.Tasks;
using Rtl433Tracker.Data.Models;

namespace Rtl433Tracker.Services.Interfaces
{
    public interface IDeviceService
    {
        Task<Guid> CreateAsync(Device device);
        Task<Device> GetAsync(string driverModel, string driverId);
        Task<Device> GetOrCreateAsync(string driverModel, string driverId);
    }
}