using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OAuth2Api.Validations;

namespace OAuth2Api.Models.Entity;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [RegularExpression(@"^\p{L}+[\p{L}\p{M}'\s-]*$",
        ErrorMessage = "First name can contain letters, diacritics, apostrophes, spaces, and hyphens.")]
    public required string FirstName { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [RegularExpression(@"^\p{L}+[\p{L}\p{M}'\s-]*$",
        ErrorMessage = "Last name can contain letters, diacritics, apostrophes, spaces, and hyphens.")]
    public required string LastName { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression(@"\d{2}-\d{2}-\d{4}",
        ErrorMessage = "Date must be in the format dd-MM-yyyy")]
    public required string DateOfBirth { get; set; }

    [Required]
    [Phone]
    [MaxLength(15)]
    public required string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(100)]
    [PasswordRequirements]
    [PasswordPropertyText]
    public required string Password { get; set; }
}