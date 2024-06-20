namespace Auth.ActiveDirectory.Models;

public record DomainDetails(string ForestName, ICollection<string> DomainControllers)
{
    public static readonly DomainDetails Null = new("N/A", []);
}