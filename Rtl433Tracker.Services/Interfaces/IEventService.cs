using System;
using System.Linq;
using System.Threading.Tasks;
using Rtl433Tracker.Data.Models;

namespace Rtl433Tracker.Services.Interfaces
{
    public interface IEventService
    {
        IQueryable<Event> GetEvents();
        Task<Guid> CreateAsync(Event eventData);
    }
}
