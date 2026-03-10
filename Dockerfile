FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["UsersAPI.sln", "."]
COPY ["src/Users.Domain/Users.Domain.csproj", "src/Users.Domain/"]
COPY ["src/Users.Application/Users.Application.csproj", "src/Users.Application/"]
COPY ["src/Users.Infrastructure/Users.Infrastructure.csproj", "src/Users.Infrastructure/"]
COPY ["src/Users.API/Users.API.csproj", "src/Users.API/"]
RUN dotnet restore "src/Users.API/Users.API.csproj"

COPY . .
RUN dotnet publish "src/Users.API/Users.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apk upgrade --no-cache
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Users.API.dll"]
