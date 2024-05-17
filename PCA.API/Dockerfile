#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY ["PrimaveraCloudAdapter/PrimaveraCloudAdapter.csproj", "PrimaveraCloudAdapter/"]
COPY ["PCA.API/PCA.API.csproj", "PCA.API/"]
COPY ["PCA.Core/PCA.Core.csproj", "PCA.Core/"]
COPY ["PCA.Configurations/PCA.Configurations.csproj", "PCA.Configurations/"]
COPY ["PCA.Infrastructure/PCA.Infrastructure.csproj", "PCA.Infrastructure/"]
RUN dotnet restore "PCA.API/PCA.API.csproj"
COPY . .
WORKDIR "/src/PCA.API"
RUN dotnet build "PCA.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PCA.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PCA.API.dll"]