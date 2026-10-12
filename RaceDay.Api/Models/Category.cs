
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(10)]
        [Column("category_type")]
        public string CategoryType { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("age_group")]
        public string? AgeGroup { get; set; }

        [Column("distance_km", TypeName = "decimal(5,2)")]
        public decimal? DistanceKm { get; set; }

        [Column("event_id")]
        public int EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; } = null!;

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }
}
