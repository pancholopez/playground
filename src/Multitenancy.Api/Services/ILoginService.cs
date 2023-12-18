using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public interface ILoginService
{
    public Task<UserAccount?> LoginAsync(string email, string password);
}