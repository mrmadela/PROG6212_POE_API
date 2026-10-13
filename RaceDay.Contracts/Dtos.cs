
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Contracts
{
    // Registration and login
    public class RegisterRequest
    {
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organiser|Participant)$")]
        public string Role { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNum { get; set; }

        [StringLength(13, MinimumLength = 13)]
        public string? IdNum { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Profile
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNum { get; set; }
        public string? IdNum { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNum { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }
    }

    // Events
    public class CreateEventRequest
    {
        [Required, StringLength(150)]
        public string EventName { get; set; } = string.Empty;

        public string? EventDescription { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required, StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 9999.99)]
        public decimal DistanceKm { get; set; }

        [Required]
        [RegularExpression("^(Run|Walk|Cycle)$")]
        public string EventType { get; set; } = string.Empty;
    }

    public class UpdateEventRequest : CreateEventRequest { }

    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? EventDescription { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int OrgId { get; set; }
    }

    // Categories
    public class CreateCategoryRequest
    {
        [Required, StringLength(150)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [RegularExpression("^(Age|Distance)$")]
        public string CategoryType { get; set; } = string.Empty;

        [StringLength(20)]
        public string? AgeGroup { get; set; }

        [Range(0.01, 999.99)]
        public decimal? DistanceKm { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryType { get; set; } = string.Empty;
        public string? AgeGroup { get; set; }
        public decimal? DistanceKm { get; set; }
        public int EventId { get; set; }
    }

    // Enrolments
    public class CreateEnrolmentRequest
    {
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

    public class EnrolmentDto
    {
        public int EnrolId { get; set; }
        public DateTime EnrolDate { get; set; }
        public int CategoryId { get; set; }
        public int ParticipantId { get; set; }
    }

    // Results
    public class CreateResultRequest
    {
        public TimeSpan? Duration { get; set; }

        [Range(0, 999.99)]
        public decimal? AverageSpeed { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }
    }

    public class ResultDto
    {
        public int ResultId { get; set; }
        public TimeSpan? Duration { get; set; }
        public decimal? AverageSpeed { get; set; }
        public int? Position { get; set; }
        public int EnrolId { get; set; }
    }
}
