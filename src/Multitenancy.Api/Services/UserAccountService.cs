using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public class UserAccountService(ApplicationDbContext dbContext) : IUserAccountService
{
    public Task<UserAccount?> AuthenticateAsync(string email, string password) 
        => dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password));
}