namespace Multitenancy.Api.Models;

public class UserAccount : ITenantAware
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid TenantId { get; set; }
}

public record LoginDto(string Email, string Password);