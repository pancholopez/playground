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

#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src
#COPY ["docker-support/DockerSupport.Web/DockerSupport.Web.csproj", "docker-support/DockerSupport.Web/"]
#RUN dotnet restore "docker-support/DockerSupport.Web/DockerSupport.Web.csproj"
#COPY . .
#WORKDIR "/src/docker-support/DockerSupport.Web"
#RUN dotnet build "DockerSupport.Web.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "DockerSupport.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DockerSupport.Web.dll"]
