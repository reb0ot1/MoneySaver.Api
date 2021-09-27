FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+;http://+&quot
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=awdzs123
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/localhost.pfx

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /usr/src
COPY ["MoneySaver.Api/MoneySaver.Api.csproj", "MoneySaver.Api/"]
COPY ["MoneySaver.Api.Data/MoneySaver.Api.Data.csproj", "MoneySaver.Api.Data/"]
COPY ["MoneySaver.Api.Models/MoneySaver.Api.Models.csproj", "MoneySaver.Api.Models/"]
COPY ["MoneySaver.Api.Services/MoneySaver.Api.Services.csproj", "MoneySaver.Api.Services/"]
RUN dotnet restore "MoneySaver.Api/MoneySaver.Api.csproj"
COPY . .
WORKDIR "/usr/src/MoneySaver.Api"
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
 