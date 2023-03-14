using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Playground.AspIdentityApp.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // always do the default setup first
        base.OnModelCreating(builder);

        //Adds Admin user definition. To take effect must add new migration and run update-database
        const string adminUserId = "3CA25620-049F-4C7D-8B88-2CEF2478C99A";
        const string adminRoleId = "DF60C8FF-48F3-45F4-B883-DF458EE206AE";

        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = adminRoleId,
            ConcurrencyStamp = adminRoleId,
            Name = "Admin",
            NormalizedName = "Admin".ToUpperInvariant()
        });

        var user = new IdentityUser
        {
            Id = adminUserId,
            ConcurrencyStamp = adminUserId,
            UserName = "admin@mail.com",
            NormalizedUserName = "admin@mail.com".ToUpperInvariant(),
            Email = "admin@mail.com",
            NormalizedEmail = "admin@mail.com".ToUpperInvariant(),
            EmailConfirmed = true
        };

        var passwordHasher = new PasswordHasher<IdentityUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, "Password");

        builder.Entity<IdentityUser>().HasData(user);

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            });
    }
}