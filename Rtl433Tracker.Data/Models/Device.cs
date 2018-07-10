using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rtl433Tracker.Data.Models
{
    public class Device
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string DriverModel { get; set; }

        [Required]
        public string DriverId { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
