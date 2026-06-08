FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/SiteYonetimi.API/SiteYonetimi.API.csproj", "src/SiteYonetimi.API/"]
COPY ["src/SiteYonetimi.Shared/SiteYonetimi.Shared.csproj", "src/SiteYonetimi.Shared/"]
COPY ["src/SiteYonetimi.Infrastructure/SiteYonetimi.Infrastructure.csproj", "src/SiteYonetimi.Infrastructure/"]
COPY ["src/Modules/SiteYonetimi.Auth/SiteYonetimi.Auth.csproj", "src/Modules/SiteYonetimi.Auth/"]
COPY ["src/Modules/SiteYonetimi.Tenancy/SiteYonetimi.Tenancy.csproj", "src/Modules/SiteYonetimi.Tenancy/"]
COPY ["src/Modules/SiteYonetimi.SiteManagement/SiteYonetimi.SiteManagement.csproj", "src/Modules/SiteYonetimi.SiteManagement/"]
RUN dotnet restore "src/SiteYonetimi.API/SiteYonetimi.API.csproj"

COPY . .
RUN dotnet build "src/SiteYonetimi.API/SiteYonetimi.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/SiteYonetimi.API/SiteYonetimi.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /app/uploads && chmod 755 /app/uploads
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SiteYonetimi.API.dll"]
