FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["CrowdSourcingService/CrowdSourcingService.csproj", "CrowdSourcingService/"]
RUN dotnet restore "CrowdSourcingService/CrowdSourcingService.csproj"
COPY . .
WORKDIR "/src/CrowdSourcingService"
RUN dotnet build "CrowdSourcingService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CrowdSourcingService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CrowdSourcingService.dll"]