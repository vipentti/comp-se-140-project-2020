# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY Intermediate/Intermediate.csproj Intermediate/
RUN dotnet restore "Intermediate/Intermediate.csproj"

# copy and publish app and libraries
COPY . .

WORKDIR /src/Intermediate
# --no-restore
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Intermediate.dll"]
