namespace Auth.Playground.AspIdentityApp.Services;

public interface IEmailService
{
    Task SendAsync(string to, string from, string subject, string body);
}