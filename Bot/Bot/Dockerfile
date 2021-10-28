FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /bot
COPY . ./
RUN dotnet publish -c Release -o build --self-contained=false