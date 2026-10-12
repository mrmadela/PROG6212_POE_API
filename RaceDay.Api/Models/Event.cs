
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("events")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("event_name")]
        public string EventName { get; set; } = string.Empty;

        [Column("event_description")]
        public string? EventDescription { get; set; }

        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(150)]
        [Column("location")]
        public string Location { get; set; } = string.Empty;

        [Column("distance_km", TypeName = "decimal(6,2)")]
        public decimal DistanceKm { get; set; }

        [Required]
        [StringLength(10)]
        [Column("event_type")]
        public string EventType { get; set; } = string.Empty;

        [Column("org_id")]
        public int OrgId { get; set; }

        [ForeignKey(nameof(OrgId))]
        public Organiser Organiser { get; set; } = null!;

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();
    }
}
