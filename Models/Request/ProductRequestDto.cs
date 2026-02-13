using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTOs;

public class ProductRequestDto : IValidatableObject
{
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title) &&
            string.IsNullOrWhiteSpace(Description) &&
            string.IsNullOrWhiteSpace(Category) &&
            (Images == null || !Images.Any()))
        {
            yield return new ValidationResult(
                "At least one of Title, Description, Category, or Images must be provided.",
                new[] { nameof(Title), nameof(Description), nameof(Category), nameof(Images) }
            );
        }
    }
}
