
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models.ViewModels
{// start of namespace

    // start of CategoryViewModel
    public class CategoryViewModel : IValidatableObject
    {
        public int CategoryId { get; set; }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(150, ErrorMessage = "Category name cannot exceed 150 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category type is required.")]
        [RegularExpression("^(Age|Distance)$",
            ErrorMessage = "Category type must be Age or Distance.")]
        public string CategoryType { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Age group cannot exceed 20 characters.")]
        public string? AgeGroup { get; set; }

        [Range(typeof(decimal), "0.01", "999.99",
            ErrorMessage = "Distance must be between 0.01 and 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        // start of category type validation
        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (CategoryType == "Age")
            {
                if (string.IsNullOrWhiteSpace(AgeGroup))
                {
                    yield return new ValidationResult(
                        "Age categories require an age group.",
                        new[] { nameof(AgeGroup) });
                }

                if (DistanceKm.HasValue)
                {
                    yield return new ValidationResult(
                        "Age categories cannot have a distance.",
                        new[] { nameof(DistanceKm) });
                }
            }
            else if (CategoryType == "Distance")
            {
                if (!DistanceKm.HasValue || DistanceKm <= 0)
                {
                    yield return new ValidationResult(
                        "Distance categories require a positive distance.",
                        new[] { nameof(DistanceKm) });
                }

                if (!string.IsNullOrWhiteSpace(AgeGroup))
                {
                    yield return new ValidationResult(
                        "Distance categories cannot have an age group.",
                        new[] { nameof(AgeGroup) });
                }
            }
        }
        // end of category type validation
    }
    // end of CategoryViewModel

}// end of namespace
