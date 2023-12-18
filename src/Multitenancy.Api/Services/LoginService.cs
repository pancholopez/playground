using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public class LoginService(ApplicationDbContext dbContext) : ILoginService
{
    public Task<UserAccount?> LoginAsync(string email, string password) 
        => dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password));
}