FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ServiceApi/ServiceApi.csproj", "src/ServiceApi/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/ServiceApi/ServiceApi.csproj"
COPY . .
WORKDIR "/src/src/ServiceApi"
RUN dotnet build "ServiceApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatHistory.ServiceApi.dll"]