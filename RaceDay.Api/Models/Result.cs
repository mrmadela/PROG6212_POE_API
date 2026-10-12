
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("results")]
    public class Result
    {
        [Key]
        [Column("result_id")]
        public int ResultId { get; set; }

        [Column("duration", TypeName = "time")]
        public TimeSpan? Duration { get; set; }

        [Column("average_speed", TypeName = "decimal(5,2)")]
        public decimal? AverageSpeed { get; set; }

        [Column("position")]
        public int? Position { get; set; }

        [Column("enrol_id")]
        public int EnrolId { get; set; }

        [ForeignKey(nameof(EnrolId))]
        public Enrolment Enrolment { get; set; } = null!;
    }
}
