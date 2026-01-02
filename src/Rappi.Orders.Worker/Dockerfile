FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Rappi.Orders.sln ./
COPY src/Rappi.Orders.Domain/Rappi.Orders.Domain.csproj src/Rappi.Orders.Domain/
COPY src/Rappi.Orders.Application/Rappi.Orders.Application.csproj src/Rappi.Orders.Application/
COPY src/Rappi.Orders.Infrastructure/Rappi.Orders.Infrastructure.csproj src/Rappi.Orders.Infrastructure/
COPY src/Rappi.Orders.Worker/Rappi.Orders.Worker.csproj src/Rappi.Orders.Worker/

RUN dotnet restore src/Rappi.Orders.Worker/Rappi.Orders.Worker.csproj

COPY . .
WORKDIR /src/src/Rappi.Orders.Worker
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
# ✅ fuerza a que los appsettings viajen (incluye Development/Staging/Production)
COPY src/Rappi.Orders.Worker/appsettings*.json ./
ENTRYPOINT ["dotnet", "Rappi.Orders.Worker.dll"]
