using System;
using System.ComponentModel.DataAnnotations;

namespace Rtl433Tracker.Data.Models
{
    public class EventData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public virtual Event Event { get; set; }

        [Required]
        public string Property { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
