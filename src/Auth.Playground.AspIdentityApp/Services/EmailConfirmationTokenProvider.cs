using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Auth.Playground.AspIdentityApp.Services;

// ReSharper disable once ClassNeverInstantiated.Global
/// <inheritdoc />
public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public EmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<EmailConfirmationTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

/// <inheritdoc />
public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    
}