FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN groupadd --system appgroup && useradd --system --gid appgroup appuser
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/UsersAPI.Api/UsersAPI.Api.csproj", "src/UsersAPI.Api/"]
RUN dotnet restore "src/UsersAPI.Api/UsersAPI.Api.csproj"

COPY . .
WORKDIR "/src/src/UsersAPI.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "UsersAPI.Api.dll"]