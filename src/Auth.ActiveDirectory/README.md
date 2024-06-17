# Active Directory Playground

## SetUp

install nuget dependencies

```bash
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.UserSecrets

# add support for Active Directory
dotnet add package System.DirectoryServices.AccountManagement
```

add support to user secrets for this project

```bash
dotnet user-secrets init
```

set a user secret

```bash
dotnet user-secrets set "ActiveDirectory:Address" "myAddress.com"
```