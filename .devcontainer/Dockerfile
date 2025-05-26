FROM alpine:3.21

ARG NODE_VERSION="~22"
ARG NPM_VERSION="~10"
ARG YARN_VERSION="~1"
ARG ASPNET_SDK_VERSION="8.0"

USER root
WORKDIR /root

# Environment
RUN mkdir -p /usr/share/dotnet/tools
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV DOTNET_ROOT="/root/.dotnet"
ENV LANG="C.UTF-8"
ENV LC_ALL="C.UTF-8"
ENV PATH="${PATH}:${DOTNET_ROOT}:${DOTNET_ROOT}/tools"

# Ensure Up To Date
RUN apk update --no-cache

# Base Dev Dependencies
RUN apk add -qu \
  bash \
  build-base \
  ca-certificates-bundle \
  curl \
  git \
  iputils \
  jq \
  openssh \
  openssh-server \
  openssl \
  tmux \
  unzip \
  vim \
  wget \
  yq

# ZSH
RUN apk add -qu zsh zsh-vcs && \
  sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" && \
  export ZSH_CUSTOM="/root/.oh-my-zsh/custom" && \
  git clone --depth 1 -- https://github.com/zsh-users/zsh-autosuggestions.git "${ZSH_CUSTOM}/plugins/zsh-autosuggestions" && \
  git clone --depth 1 -- https://github.com/zsh-users/zsh-syntax-highlighting.git "${ZSH_CUSTOM}/plugins/zsh-syntax-highlighting" && \
  sed -i 's/plugins=(git)/plugins=(git zsh-autosuggestions zsh-syntax-highlighting)/g' .zshrc && \
  sed -i 's/ZSH_THEME="robbyrussell"/ZSH_THEME="agnoster"/g' .zshrc && \
  cp ~/.oh-my-zsh/themes/agnoster.zsh-theme ~/.oh-my-zsh/custom/themes/ && \
  sed -i '/^prompt_context()/,/^}/c\prompt_context() {}' ~/.oh-my-zsh/custom/themes/agnoster.zsh-theme

# Workspace (Following VS Code)
RUN mkdir /workspaces && \
  chown root:root /workspaces && \
  chmod 775 /workspaces

# Node.js
RUN apk add -qu "nodejs=${NODE_VERSION}" "npm=${NPM_VERSION}" "yarn=${YARN_VERSION}" && \
  echo "Node Version: $(node -v)" && \
  echo "NPM Version: $(npm -v)" && \
  echo "Yarn Version: $(yarn -v)" && \
  npm i -g vsce && \
  npm i -g npm-check-updates

# .NET
RUN apk add -qu icu-libs libgcc libssl3 libstdc++ libgdiplus zlib && \
  wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && \
  chmod +x ./dotnet-install.sh && \
  ./dotnet-install.sh --channel "$ASPNET_SDK_VERSION" --install-dir "$DOTNET_ROOT" && \
  echo dotnet version: $(dotnet --version) && \
  rm dotnet-install.sh

# .NET Tools
RUN dotnet tool install --global dotnet-coverage && \
  dotnet tool install --global dotnet-reportgenerator-globaltool && \
  dotnet tool install --global dotnet-ef

# Docker
RUN apk add -qu docker-cli-buildx

# Kubectl
RUN curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl" && \
  curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl.sha256" && \
  echo "$(cat kubectl.sha256) kubectl" | sha256sum -c && \
  install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl && \
  echo "Kubectl Version: $(kubectl version --client --output yaml | yq '.clientVersion.gitVersion')" && \
  rm kubectl kubectl.sha256

# Helm
RUN curl -fsSL https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 -o get_helm.sh && \
  chmod 700 get_helm.sh && \
  ./get_helm.sh && \
  rm get_helm.sh && \
  helm repo add bitnami https://charts.bitnami.com/bitnami && \
  helm repo add nginx https://kubernetes.github.io/ingress-nginx

# Runtime
COPY .devcontainer/postCreateCommand.sh /usr/local/bin/postCreateCommand
RUN chmod +x /usr/local/bin/postCreateCommand

USER root
WORKDIR /workspaces
