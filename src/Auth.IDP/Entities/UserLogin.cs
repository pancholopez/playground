using System.ComponentModel.DataAnnotations;

namespace Auth.IDP.Entities;

public class UserLogin : IConcurrencyAware
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Provider { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string ProviderIdentityKey { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public User User { get; set; }

    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
}