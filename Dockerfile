FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TelegramSendArtifactsAction/TelegramSendArtifactsAction.csproj", "TelegramSendArtifactsAction/"]
RUN dotnet restore "TelegramSendArtifactsAction/TelegramSendArtifactsAction.csproj"
COPY . .
WORKDIR "/src/TelegramSendArtifactsAction"
RUN dotnet build "TelegramSendArtifactsAction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramSendArtifactsAction.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramSendArtifactsAction.dll"]
