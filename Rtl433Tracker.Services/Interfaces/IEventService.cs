using System;
using System.Threading.Tasks;
using Rtl433Tracker.Data.Models;

namespace Rtl433Tracker.Services.Interfaces
{
    public interface IEventService
    {
        Task<Guid> CreateAsync(Event eventData);
    }
}
