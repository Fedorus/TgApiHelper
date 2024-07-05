#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal-arm64v8 AS build
WORKDIR /src
ENV NUGET_XMLDOC_MODE=none
COPY ["TelegramFaqBotHost/TelegramFaqBotHost.csproj", "TelegramFaqBotHost/"]
RUN dotnet restore "TelegramFaqBotHost/TelegramFaqBotHost.csproj"
COPY . .
WORKDIR "/src/TelegramFaqBotHost"
RUN dotnet build "TelegramFaqBotHost.csproj" -c Release -o /app/build


FROM build AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TelegramFaqBotHost.dll"]