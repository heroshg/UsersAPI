FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
RUN groupadd --system appgroup && useradd --system --gid appgroup appuser

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/UsersAPI.Api/UsersAPI.Api.csproj", "UsersAPI.Api/"]
RUN dotnet restore "UsersAPI.Api/UsersAPI.Api.csproj"
COPY src/UsersAPI.Api/ UsersAPI.Api/
WORKDIR "/src/UsersAPI.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "UsersAPI.Api.dll"]