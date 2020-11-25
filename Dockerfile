ARG BUILD_CONFIG=Release
ARG TargetProject

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime
WORKDIR /app

# Install tini https://github.com/krallin/tini
ENV TINI_VERSION v0.19.0
ADD https://github.com/krallin/tini/releases/download/${TINI_VERSION}/tini /tini
RUN chmod +x /tini

EXPOSE 80
# Run the actual application under Tini
ENTRYPOINT ["/tini", "-vv", "--"]

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
ARG BUILD_CONFIG

COPY ./amqp.sln ./
COPY ./src ./src
COPY ./tests ./tests

RUN dotnet restore "./amqp.sln"
RUN dotnet build "./amqp.sln" -c ${BUILD_CONFIG} --no-restore

FROM build as publish
WORKDIR /src
ARG BUILD_CONFIG
ARG TargetProject

RUN dotnet publish "./src/${TargetProject}/${TargetProject}.csproj" -c ${BUILD_CONFIG} -o /app/publish --no-build

# final stage/image
FROM runtime as final
WORKDIR /app
ARG TargetProject
ENV EnvTargetProject=${TargetProject}
COPY --from=publish /app/publish .

# Create a script to run the specified TargetProject
RUN echo "#!/bin/sh\nexec dotnet ${TargetProject}.dll" > entry.sh \
    && chmod +x ./entry.sh

# NOTE: Using CMD here since the entrypoint using tini
# is defined in the runtime base image
CMD [ "./entry.sh" ]
