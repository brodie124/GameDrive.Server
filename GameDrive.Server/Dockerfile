FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GameDrive.Server.csproj", "GameDrive.Server/"]
COPY ["../GameDrive.Server.Migrations.MySQL/", "GameDrive.Server.Migrations.MySQL/"]
COPY ["../GameDrive.Server.Migrations.SQLite/", "GameDrive.Server.Migrations.SQLite/"]
COPY ["../GameDrive.Server.Domain/", "GameDrive.Server.Domain"]
COPY . .

WORKDIR "/src/GameDrive.Server"

RUN dotnet restore "GameDrive.Server.csproj"
RUN dotnet build "GameDrive.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameDrive.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameDrive.Server.dll"]
