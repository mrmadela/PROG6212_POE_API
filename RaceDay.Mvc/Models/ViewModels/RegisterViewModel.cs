
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models.ViewModels
{// start of namespace
    // start of class
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant";

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNum { get; set; }

        public string? IdNum { get; set; }

        public string? Address { get; set; }
    }
    // end of class
}// end of namespace
