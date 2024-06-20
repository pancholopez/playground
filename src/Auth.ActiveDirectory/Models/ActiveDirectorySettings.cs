namespace Auth.ActiveDirectory.Models;

public record ActiveDirectorySettings(
    string ServerName,
    string UserName,
    string Password,
    int TimeOutInMilliSeconds);