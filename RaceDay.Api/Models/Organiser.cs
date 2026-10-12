
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{// start of namespace 
    [Table("organisers")]
    public class Organiser
    { //start of class
        [Key]
        [Column("org_id")]
        public int OrgId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("phone_num")]
        public string PhoneNum { get; set; } = string.Empty;

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }// end of class
}// end of namespace
