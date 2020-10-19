# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY HttpServer/HttpServer.csproj HttpServer/
RUN dotnet restore "HttpServer/HttpServer.csproj"

# copy and publish app and libraries
COPY . .

WORKDIR /src/HttpServer
# --no-restore
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "HttpServer.dll"]
