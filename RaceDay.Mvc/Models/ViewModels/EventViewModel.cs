using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models.ViewModels
{// start of namespace

    public class EventViewModel
    {// start of class

        public int EventId { get; set; }

        [Required(ErrorMessage = "Please enter the event name.")]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? EventDescription { get; set; }

        [Required(ErrorMessage = "Please select the event date.")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter the event location.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0.1, 1000, ErrorMessage = "Distance must be greater than zero.")]
        public decimal DistanceKm { get; set; }

        [Required(ErrorMessage = "Please select an event type.")]
        public string EventType { get; set; } = string.Empty;

    }// end of class

}// end of namespace
