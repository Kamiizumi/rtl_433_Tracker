using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rtl433Tracker.Data.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public virtual Device Device { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public virtual ICollection<EventData> Data { get; set; }
    }
}
