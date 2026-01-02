FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Rappi.Orders.sln ./
COPY src/Rappi.Orders.Domain/Rappi.Orders.Domain.csproj src/Rappi.Orders.Domain/
COPY src/Rappi.Orders.Application/Rappi.Orders.Application.csproj src/Rappi.Orders.Application/
COPY src/Rappi.Orders.Infrastructure/Rappi.Orders.Infrastructure.csproj src/Rappi.Orders.Infrastructure/
COPY src/Rappi.Orders.Api/Rappi.Orders.Api.csproj src/Rappi.Orders.Api/

RUN dotnet restore src/Rappi.Orders.Api/Rappi.Orders.Api.csproj

COPY . .
WORKDIR /src/src/Rappi.Orders.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Rappi.Orders.Api.dll"]
