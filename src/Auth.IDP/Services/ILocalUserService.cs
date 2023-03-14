using System.Security.Claims;
using Auth.IDP.Entities;

namespace Auth.IDP.Services;

public interface ILocalUserService
{
    Task<UserSecret> GetUserSecretAsync(string subject, string name);

    Task<bool> AddUserSecret(string subject, string name, string secret);
    
    Task<bool> ValidateCredentialsAsync(string userName, string password);

    Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(string subject);
    
    Task<User> GetUserByEmailAsync(string email);
    
    Task AddExternalProviderToUser(string subject, string provider, string providerIdentityKey);

    Task<User> GetUserByUserNameAsync(string userName);

    Task<User> GetUserBySubjectAsync(string subject);

    void AddUser(User userToAdd, string password);

    Task<bool> IsUserActive(string subject);
    
    Task<bool> ActivateUserAsync(string securityCode);

    Task<User> FindByExternalProvider(string provider, string providerIdentityKey);

    User AutoProvisionUser(string provider, string providerIdentityKey, IEnumerable<Claim> claims);

    Task<bool> SaveChangesAsync();
}