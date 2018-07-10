using Microsoft.EntityFrameworkCore;
using Rtl433Tracker.Data.Models;

namespace Rtl433Tracker.Data
{
    public class Rtl433TrackerContext : DbContext
    {
        public Rtl433TrackerContext(DbContextOptions<Rtl433TrackerContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
    }
}
