FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY LeoMadeiras.sln .
COPY src/LeoMadeiras.API/LeoMadeiras.API.csproj                       src/LeoMadeiras.API/
COPY src/LeoMadeiras.Application/LeoMadeiras.Application.csproj       src/LeoMadeiras.Application/
COPY src/LeoMadeiras.Domain/LeoMadeiras.Domain.csproj                 src/LeoMadeiras.Domain/
COPY src/LeoMadeiras.Infrastructure/LeoMadeiras.Infrastructure.csproj src/LeoMadeiras.Infrastructure/
COPY tests/LeoMadeiras.Tests/LeoMadeiras.Tests.csproj                 tests/LeoMadeiras.Tests/
RUN dotnet restore
COPY src/ src/
COPY tests/ tests/
RUN dotnet publish src/LeoMadeiras.API -c Release -o /out --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "LeoMadeiras.API.dll"]