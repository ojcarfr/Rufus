FROM mcr.microsoft.com/dotnet/nightly/sdk:11.0.100-rc.2
ENV PATH=$PATH:/home/vscode/.dotnet:/home/vscode/.dotnet/tools

# Let common-utils create its configured vscode user from the base image's
# existing UID 1000 ubuntu user.
RUN userdel -f -r ubuntu

# Keep the .NET 11 preview from the base image and add the released SDKs.
COPY dotnet-install.sh /tmp/dotnet-install.sh
RUN sed -i 's/\r$//' /tmp/dotnet-install.sh \
    && chmod +x /tmp/dotnet-install.sh \
    && /tmp/dotnet-install.sh 8.0.425 10.0.401 \
    && rm /tmp/dotnet-install.sh

RUN chmod -R a+rwX /usr/share/dotnet

# clear this environment variable so xml docs from NuGet packages are unpackaged. The default dotnet/sdk image sets it to 'skip'.
# see https://github.com/dotnet/dotnet-docker/issues/2790
ENV NUGET_XMLDOC_MODE=