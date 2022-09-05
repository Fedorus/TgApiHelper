#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal-arm64v8 AS build
WORKDIR /src
COPY ["TelegramFaqBotHost/TelegramFaqBotHost.csproj", "TelegramFaqBotHost/"]
RUN dotnet restore "TelegramFaqBotHost/TelegramFaqBotHost.csproj"
COPY . .
WORKDIR "/src/TelegramFaqBotHost"
RUN dotnet build "TelegramFaqBotHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramFaqBotHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramFaqBotHost.dll"]