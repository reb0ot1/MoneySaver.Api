
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore "MoneySaver.Api/MoneySaver.Api.csproj"
# Build and publish a release
RUN dotnet publish "MoneySaver.Api/MoneySaver.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MoneySaver.Api.dll"]
# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /usr/src
# COPY . ./
# # COPY ["MoneySaver.Api/MoneySaver.Api.csproj", "MoneySaver.Api/"]
# # COPY ["MoneySaver.Api.Data/MoneySaver.Api.Data.csproj", "MoneySaver.Api.Data/"]
# # COPY ["MoneySaver.Api.Models/MoneySaver.Api.Models.csproj", "MoneySaver.Api.Models/"]
# # COPY ["MoneySaver.Api.Services/MoneySaver.Api.Services.csproj", "MoneySaver.Api.Services/"]
# RUN dotnet restore "MoneySaver.Api/MoneySaver.Api.csproj"

# WORKDIR "/usr/src/MoneySaver.Api"
# RUN dotnet build "MoneySaver.Api.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "MoneySaver.Api.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "MoneySaver.Api.dll"]
 