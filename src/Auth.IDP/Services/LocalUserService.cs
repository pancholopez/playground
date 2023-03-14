using System.Security.Claims;
using System.Security.Cryptography;
using Auth.IDP.DbContexts;
using Auth.IDP.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.IDP.Services;

public class LocalUserService : ILocalUserService
{
    private readonly IdentityDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LocalUserService(
        IdentityDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context ??
                   throw new ArgumentNullException(nameof(context));
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> IsUserActive(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return false;
        }

        var user = await GetUserBySubjectAsync(subject);

        if (user == null)
        {
            return false;
        }

        return user.Active;
    }

    public async Task<bool> AddUserSecret(string subject, string name, string secret)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(secret);
        
        var user = await GetUserBySubjectAsync(subject);

        if (user == null)
        {
            return false;
        }

        user.UserSecrets.Add(new UserSecret() 
            { Name = name, Secret = secret });
        return true;
    }

    public async Task<UserSecret> GetUserSecretAsync(
        string subject, string name)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(name);

        return await _context.UserSecrets
            .FirstOrDefaultAsync(u => u.User.Subject == subject && u.Name == name);
    }

    public async Task<bool> ValidateCredentialsAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var user = await GetUserByUserNameAsync(userName);

        if (user == null)
        {
            return false;
        }

        if (!user.Active)
        {
            return false;
        }

        // Validate credentials
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return verificationResult == PasswordVerificationResult.Success;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        if (email is null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddExternalProviderToUser(string subject, string provider, string providerIdentityKey)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentNullException(nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(providerIdentityKey))
        {
            throw new ArgumentNullException(nameof(providerIdentityKey));
        }

        var user = await GetUserBySubjectAsync(subject);
        user.Logins.Add(new UserLogin()
        {
            Provider = provider,
            ProviderIdentityKey = providerIdentityKey
        });
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentNullException(nameof(userName));
        }

        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentNullException(nameof(subject));
        }

        return await _context.UserClaims.Where(u => u.User.Subject == subject).ToListAsync();
    }

    public async Task<User> GetUserBySubjectAsync(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentNullException(nameof(subject));
        }

        return await _context.Users.FirstOrDefaultAsync(u => u.Subject == subject);
    }

    public void AddUser(User userToAdd, string password)
    {
        if (userToAdd == null)
        {
            throw new ArgumentNullException(nameof(userToAdd));
        }

        if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
        {
            // in a real-life scenario you'll probably want to 
            // return this as a validation issue
            throw new Exception("Username must be unique");
        }

        if (_context.Users.Any(u => u.Email == userToAdd.Email))
        {
            throw new Exception("Email must be unique");
        }

        userToAdd.SecurityCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        userToAdd.SecureCodeExpirationDate = DateTime.UtcNow.AddHours(1);

        // hash password
        userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);

        _context.Users.Add(userToAdd);
    }

    public async Task<bool> ActivateUserAsync(string securityCode)
    {
        if (string.IsNullOrWhiteSpace(securityCode))
        {
            throw new ArgumentNullException(nameof(securityCode));
        }

        var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.SecurityCode.Equals(securityCode) && x.SecureCodeExpirationDate >= DateTime.UtcNow);

        if (user == null)
        {
            return false;
        }

        user.Active = true;
        user.SecurityCode = null;
        return true;
    }

    public async Task<User> FindByExternalProvider(string provider, string providerIdentityKey)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentNullException(nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(providerIdentityKey))
        {
            throw new ArgumentNullException(nameof(providerIdentityKey));
        }

        var userLogin = await _context.UserLogins.Include(ul => ul.User)
            .FirstOrDefaultAsync(ul =>
                ul.Provider.Equals(provider) 
                && ul.ProviderIdentityKey.Equals(providerIdentityKey));

        return userLogin?.User;
    }

    public User AutoProvisionUser(string provider, string providerIdentityKey, IEnumerable<Claim> claims)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentNullException(nameof(provider));
        }
        
        if (string.IsNullOrWhiteSpace(providerIdentityKey))
        {
            throw new ArgumentNullException(nameof(providerIdentityKey));
        }
        
        if (claims is null)
        {
            throw new ArgumentNullException(nameof(claims));
        }

        var user = new User
        {
            Active = true,
            Subject = Guid.NewGuid().ToString()
        };

        foreach (var claim in claims)
        {
            user.Claims.Add(new UserClaim
            {
                Type = claim.Type,
                Value = claim.Value
            });
        }
        
        user.Logins.Add(new UserLogin
        {
            Provider = provider,
            ProviderIdentityKey = providerIdentityKey
        });

        _context.Users.Add(user);

        return user;
    }

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
}