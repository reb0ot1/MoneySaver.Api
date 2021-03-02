FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MoneySaver.Api/MoneySaver.Api/MoneySaver.Api.csproj", "MoneySaver.Api/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Data/MoneySaver.Api.Data.csproj", "MoneySaver.Api.Data/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Services/MoneySaver.Api.Services.csproj", "MoneySaver.Api.Services/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Models/MoneySaver.Api.Models.csproj", "MoneySaver.Api.Models/"]
RUN dotnet restore "MoneySaver.Api/MoneySaver.Api.csproj"
COPY ["MoneySaver.Api/MoneySaver.Api/.", "MoneySaver.Api/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Data/.", "MoneySaver.Api.Data/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Services/.", "MoneySaver.Api.Services/"]
COPY ["MoneySaver.Api/MoneySaver.Api.Models/.", "MoneySaver.Api.Models/"]
WORKDIR "/src/MoneySaver.Api"
RUN dotnet build "MoneySaver.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoneySaver.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["testcert/localhost.pfx", "/https/localhost.pfx"]
COPY ["testcert/RootCA.crt", "/usr/local/share/ca-certificates/RootCA.crt"]
RUN update-ca-certificates
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneySaver.Api.dll"]