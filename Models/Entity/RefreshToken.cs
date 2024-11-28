using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth2Api.Models.Entity;

public sealed class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(88)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MaxLength(36)]
    public string JwtId { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    public bool Used { get; set; }

    public bool Invalidated { get; set; }

    public DateTime? RevokedAt { get; set; }

    [MaxLength(88)]
    public string? ReplacedByToken { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

    public bool IsActive => !Used && !Invalidated && !IsExpired && RevokedAt == null;
} 