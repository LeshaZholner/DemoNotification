FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

WORKDIR /src/DemoNotification.NotificationSendService
RUN dotnet publish DemoNotification.NotificationSendService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DemoNotification.NotificationSendService.dll"]