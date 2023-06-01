# build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /docker-support

# restore
COPY ["DockerSupport.Web/DockerSupport.Web.csproj", "DockerSupport.Web/"]
RUN dotnet restore "DockerSupport.Web"

COPY ["DockerSupport.UnitTests/DockerSupport.UnitTests.csproj", "DockerSupport.UnitTests/"]
RUN dotnet restore "DockerSupport.UnitTests"

# copy source
COPY . .

# run unit tests
RUN dotnet test "DockerSupport.UnitTests" --no-restore  -c Release

# publish
FROM build as publish
RUN dotnet publish "DockerSupport.Web/DockerSupport.Web.csproj" -c Release -o /app /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DockerSupport.Web.dll"]