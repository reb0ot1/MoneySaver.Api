FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MoneySaver.Api/MoneySaver.Api.csproj", "MoneySaver.Api/"]
COPY ["MoneySaver.Api.Data/MoneySaver.Api.Data.csproj", "MoneySaver.Api.Data/"]
COPY ["MoneySaver.Api.Services/MoneySaver.Api.Services.csproj", "MoneySaver.Api.Services/"]
RUN dotnet restore "MoneySaver.Api/MoneySaver.Api.csproj"
COPY . .
WORKDIR "/src/MoneySaver.Api"
RUN dotnet build "MoneySaver.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoneySaver.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneySaver.Api.dll"]