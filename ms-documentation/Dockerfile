#Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update && apt-get install -y \
    curl \
    htop \
    iputils-ping \
    libfontconfig1 \
    fonts-dejavu \
    fontconfig \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

#Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled-extra
WORKDIR /app

COPY --from=build /usr/share/fonts /usr/share/fonts
COPY --from=build /etc/fonts/fonts.conf /etc/fonts/fonts.conf


COPY --from=build /app/publish .

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_URLS=http://+:5111

EXPOSE 5111

USER 1000:1000

ENTRYPOINT ["dotnet", "ms-documentation.dll"]