namespace Auth.ActiveDirectory.Models;

public record NewUserAccount(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    DateTime? ExpirationDate = null
)
{
    public string FullName => $"{FirstName} {LastName}";
}