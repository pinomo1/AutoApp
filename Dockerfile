# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["AutoApp.API/AutoApp.API.csproj", "AutoApp.API/"]
COPY ["AutoApp.Application/AutoApp.Application.csproj", "AutoApp.Application/"]
COPY ["AutoApp.Domain/AutoApp.Domain.csproj", "AutoApp.Domain/"]
COPY ["AutoApp.Infrastructure/AutoApp.Infrastructure.csproj", "AutoApp.Infrastructure/"]

RUN dotnet restore "AutoApp.API/AutoApp.API.csproj"

# Copy the rest of the source and publish the API
COPY . .
WORKDIR /src/AutoApp.API
RUN dotnet publish "AutoApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
# Install ICU and timezone data for full globalization support on Alpine
RUN apk add --no-cache icu-libs tzdata \
    && cp /usr/share/zoneinfo/UTC /etc/localtime || true
# Ensure .NET uses ICU (not invariant) so CultureInfo works as expected
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Create logs directory with world-writable permissions for Serilog file sink
RUN mkdir -p /app/logs && chmod 777 /app/logs

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AutoApp.API.dll"]
