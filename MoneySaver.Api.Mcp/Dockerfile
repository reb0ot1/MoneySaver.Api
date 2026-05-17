
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore "MoneySaver.Api.Mcp.csproj"
# Build and publish a release
RUN dotnet publish "MoneySaver.Api.Mcp.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MoneySaver.Api.Mcp.dll"]
 