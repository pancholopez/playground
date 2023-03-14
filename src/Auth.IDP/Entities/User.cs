using System.ComponentModel.DataAnnotations;

namespace Auth.IDP.Entities;

public class User : IConcurrencyAware
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(200)]
    [Required]
    public string Subject { get; set; }

    [MaxLength(200)]
    public string UserName { get; set; }

    [MaxLength(200)]
    public string Password { get; set; }

    [Required]
    public bool Active { get; set; }
    
    [MaxLength(200)]
    public string Email { get; set; }
    
    [MaxLength(200)]
    public string SecurityCode { get; set; }

    public DateTime SecureCodeExpirationDate { get; set; }

    [ConcurrencyCheck]
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

    public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();

    public ICollection<UserSecret> UserSecrets { get; set; } = new List<UserSecret>();
}