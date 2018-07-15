using Rtl433Tracker.Data;
using Rtl433Tracker.Data.Models;
using Rtl433Tracker.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rtl433Tracker.Services
{
    public class EventService : IEventService
    {
        private readonly Rtl433TrackerContext _rtl433TrackerContext;

        public EventService(Rtl433TrackerContext rtl433TrackerContext)
        {
            _rtl433TrackerContext = rtl433TrackerContext ?? throw new ArgumentNullException(nameof(rtl433TrackerContext));
        }

        public async Task<Guid> CreateAsync(Event eventData)
        {
            await _rtl433TrackerContext
                .Events
                .AddAsync(eventData);
            await _rtl433TrackerContext.SaveChangesAsync();

            return eventData.Id;
        }
    }
}
