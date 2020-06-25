FROM mcr.microsoft.com/iotedge/iotedgedev:latest

WORKDIR /tmp

# Install Microsoft Package Repositories
RUN wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb && \
    sudo dpkg -i packages-microsoft-prod.deb && \
    sudo apt-get update

# Install .NET Core 3.1
RUN sudo apt-get install -y apt-transport-https && \
    sudo apt-get install -y dotnet-sdk-3.1

# Install the Azure Functions Core Tools
RUN sudo apt-get install azure-functions-core-tools-3

# Install the azure CLI IoT Externsions
RUN sudo az extension add -n azure-iot
