FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AverbacaoService/src/AverbacaoService.csproj", "AverbacaoService/src/"]
RUN dotnet restore "AverbacaoService/src/AverbacaoService.csproj"
COPY . .
WORKDIR "/src/AverbacaoService/src"
RUN dotnet build "./AverbacaoService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AverbacaoService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AverbacaoService.dll"]
