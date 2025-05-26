ARG BASE_REGISTRY=docker.io/library
ARG BASE_IMAGE=alpine
ARG BASE_TAG=3.21

FROM ${BASE_REGISTRY}/${BASE_IMAGE}:${BASE_TAG}

ARG USERNAME=media
ARG GROUPNAME=media
ARG UID=1001
ARG GID=1001

ARG APP_ARCHIVE=media-pilot.tar.gz
ARG DOTNET_FILE=aspnetcore-runtime-linux-musl-x64.tar.gz

# .NET Environment Variables
ENV DOTNET_ROOT=/dotnet
ENV PATH=$PATH:${DOTNET_ROOT}:${DOTNET_ROOT}/tools
ENV DOTNET_URLS=http://*:5000
ENV DOTNET_EnableDiagnostics=0

USER root

# .NET dependencies + dumb-init + bash
RUN apk add -qu --no-cache \
  bash \
  dumb-init \
  ca-certificates-bundle \
  icu-libs \
  libgcc \
  libssl3 \
  libstdc++ \
  zlib

# setup user
RUN addgroup --gid ${GID} ${GROUPNAME} && \
  adduser --ingroup ${GROUPNAME} --disabled-password --no-create-home --uid ${UID} ${USERNAME} && \
  chown root:root /usr/bin/dumb-init

WORKDIR /tmp

# setup .NET
COPY ${DOTNET_FILE} ./

RUN mkdir ${DOTNET_ROOT} && \
  tar -zxf ${DOTNET_FILE} -C ${DOTNET_ROOT} && \
  chown -R ${UID}:${GID} ${DOTNET_ROOT} && \
  rm ${DOTNET_FILE}

# setup the app
COPY LICENSE ${APP_ARCHIVE} ./

RUN mkdir /app && \
  tar -zxf ${APP_ARCHIVE} -C /app && \
  mv LICENSE /app && \
  chown -R ${UID}:${GID} /app/* && \ 
  rm -rf ./*

WORKDIR /app
USER ${UID}:${GID}

ENTRYPOINT ["/usr/bin/dumb-init", "--"]
CMD ["dotnet", "MediaPilot.Server.dll"]

# port to expose, should mirror DOTNET_URLS
EXPOSE 5000
