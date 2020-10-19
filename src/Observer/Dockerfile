# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY Observer/Observer.csproj Observer/
RUN dotnet restore "Observer/Observer.csproj"

# copy and publish app and libraries
COPY . .

WORKDIR /src/Observer
# --no-restore
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Observer.dll"]
