FROM ubuntu:18.04

# Install prerequisites
RUN apt-get update
RUN apt-get install -y \
    wget \
    openjdk-11-jre \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg-agent \
    software-properties-common

# Install Dotnet Core 3.1
RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN add-apt-repository universe
RUN apt-get update
RUN apt-get install apt-transport-https -y
RUN apt-get update
RUN apt-get install dotnet-sdk-3.1 -y
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-sonarscanner

RUN mkdir /builds
RUN mkdir /builds/ksitarek
RUN mkdir /builds/ksitarek/vtb/
RUN mkdir /builds/ksitarek/vtb/.nuget
RUN ln -sf /builds/ksitarek/vtb/.nuget /root/.nuget/packages
ENV NUGET_PACKAGES=/builds/ksitarek/vtb/.nuget

CMD ["/bin/bash"]