FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

# Remove the Yarn repository since we do not have the repository's GPG in the base image.
# If we don't remove the repository, calls to `apt-get update` will fail, preventing
# installation of any new packages, such as those installed by devcontainer features.
RUN rm -f /etc/apt/sources.list.d/yarn.list && \
    apt-get update
