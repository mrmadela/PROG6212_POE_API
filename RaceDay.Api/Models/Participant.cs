
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{// start of namespace 
    [Table("participants")]
    public class Participant
    {// start of class
        [Key]
        [Column("participant_id")]
        public int ParticipantId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(13)]
        [Column("id_num")]
        public string IdNum { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("address")]
        public string? Address { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }// end of class
}// end of namespace
