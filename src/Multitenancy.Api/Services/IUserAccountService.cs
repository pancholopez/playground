using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public interface IUserAccountService
{
    public Task<UserAccount?> AuthenticateAsync(string email, string password);
}