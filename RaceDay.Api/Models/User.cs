
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{// start of namespace
    [Table("users")]
    public class User
    {// start of class
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        // The role is Organiser or Participant.
        [Required]
        [StringLength(20)]
        [Column("role")]
        public string Role { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }// end of class
}// end of namespace
