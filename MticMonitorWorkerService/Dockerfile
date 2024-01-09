FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MticMonitorWorkerService/MticMonitorWorkerService.csproj", "MticMonitorWorkerService/"]
RUN dotnet restore "MticMonitorWorkerService/MticMonitorWorkerService.csproj"
COPY . .
WORKDIR "/src/MticMonitorWorkerService"
RUN dotnet build "MticMonitorWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MticMonitorWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MticMonitorWorkerService.dll"]
