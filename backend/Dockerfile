FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FleetPulse.API/FleetPulse.API.csproj", "FleetPulse.API/"]
RUN dotnet restore "FleetPulse.API/FleetPulse.API.csproj"

COPY . .
RUN dotnet build "FleetPulse.API/FleetPulse.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FleetPulse.API/FleetPulse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y libfontconfig1 && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FleetPulse.API.dll"]
