
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("enrolments")]
    public class Enrolment
    {
        [Key]
        [Column("enrol_id")]
        public int EnrolId { get; set; }

        [Column("enrol_date", TypeName = "date")]
        public DateTime EnrolDate { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [Column("participant_id")]
        public int ParticipantId { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public Participant Participant { get; set; } = null!;

        // A result is optional until the event has been completed.
        public Result? Result { get; set; }
    }
}
