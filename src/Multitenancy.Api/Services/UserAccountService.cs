using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public class UserAccountService(ApplicationDbContext dbContext) : IUserAccountService
{
    public Task<UserAccount?> AuthenticateAsync(string email, string password) 
        => dbContext
            .UserAccounts
            .IgnoreQueryFilters()   // on login we dont know the tenant
            .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password));
}