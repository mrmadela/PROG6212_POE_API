using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models.ViewModels
{// START OF NAMESPACE

    // START OF RESULT VIEWMODEL
    public class ResultViewModel : IValidatableObject
    {
        public int EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Enrolment ID must be valid.")]
        public int EnrolId { get; set; }

        [Display(Name = "Finish Duration")]
        public TimeSpan? Duration { get; set; }

        [Display(Name = "Average Speed (km/h)")]
        [Range(typeof(decimal), "0", "999.99",
            ErrorMessage = "Average speed must be between 0 and 999.99.")]
        public decimal? AverageSpeed { get; set; }

        [Display(Name = "Finishing Position")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Position must be at least 1.")]
        public int? Position { get; set; }

        // START OF CUSTOM VALIDATION
        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (!Duration.HasValue &&
                !AverageSpeed.HasValue &&
                !Position.HasValue)
            {
                yield return new ValidationResult(
                    "Enter at least one result detail.",
                    new[]
                    {
                        nameof(Duration),
                        nameof(AverageSpeed),
                        nameof(Position)
                    });
            }

            if (Duration.HasValue && Duration.Value < TimeSpan.Zero)
            {
                yield return new ValidationResult(
                    "Duration cannot be negative.",
                    new[] { nameof(Duration) });
            }
        }
        // END OF CUSTOM VALIDATION
    }
    // END OF RESULT VIEWMODEL

}// END OF NAMESPACE