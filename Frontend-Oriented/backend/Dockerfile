FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Backend.sln ./
COPY Backend.Domain/Backend.Domain.csproj Backend.Domain/
COPY Backend.Infrastructure/Backend.Infrastructure.csproj Backend.Infrastructure/
COPY Backend.Api/Backend.Api.csproj Backend.Api/
RUN dotnet restore Backend.sln

COPY . .
RUN dotnet publish Backend.Api/Backend.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
USER $APP_UID

ENTRYPOINT ["dotnet", "Backend.Api.dll"]
