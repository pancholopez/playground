#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DockerSupport.WebApi/DockerSupport.WebApi.csproj", "src/DockerSupport.WebApi/"]
RUN dotnet restore "src/DockerSupport.WebApi/DockerSupport.WebApi.csproj"
COPY . .
WORKDIR "/src/src/DockerSupport.WebApi"
RUN dotnet build "DockerSupport.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerSupport.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerSupport.WebApi.dll"]