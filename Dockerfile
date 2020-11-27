ARG BUILD_CONFIG=Release
ARG TargetProject

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime
WORKDIR /app


# ENV TINI_VERSION v0.19.0
# ADD https://github.com/krallin/tini/releases/download/${TINI_VERSION}/tini /tini

# Install tini https://github.com/krallin/tini
COPY ./DevOps/scripts/tini /tini
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

# Test runner target
# Provides an additional entrypoint that can be specifically targeted
# using --target testrunner when building the image
FROM build as testrunner
WORKDIR /src
ARG BUILD_CONFIG
ENV ENV_BUILD_CONFIG=$BUILD_CONFIG
COPY ./DevOps/scripts/run-tests.ps1 ./DevOps/scripts/run-tests.ps1
COPY ./DevOps/scripts/invoke-script.ps1 ./DevOps/scripts/invoke-script.ps1
RUN chmod +x ./DevOps/scripts/*.ps1
ENTRYPOINT ["./DevOps/scripts/run-tests.ps1"]

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

# RUN apt-get update && apt-get install -y --no-install-recommends netcat

COPY --from=publish /app/publish .

# Create a script to run the specified TargetProject
# This utilizes the build time argument TargetProjeect
RUN echo "#!/bin/sh\nexec dotnet ${TargetProject}.dll" > entry.sh \
    && chmod +x ./entry.sh

COPY ./DevOps/scripts/wait-for-tcp.sh ./
RUN chmod +x ./wait-for-tcp.sh

# NOTE: Using CMD here since the entrypoint using tini
# is defined in the runtime base image
CMD [ "./entry.sh" ]
