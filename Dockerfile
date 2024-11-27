#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ShareNote.API/ShareNote.API.csproj", "ShareNote.API/"]
COPY ["ShareNote.Application/ShareNote.Application.csproj", "ShareNote.Application/"]
COPY ["ShareNote.Domain/ShareNote.Domain.csproj", "ShareNote.Domain/"]
COPY ["ShareNote.Infrasstructure/ShareNote.Infrasstructure.csproj", "ShareNote.Infrasstructure/"]
RUN dotnet restore "./ShareNote.API/ShareNote.API.csproj"
COPY . .
WORKDIR "/src/ShareNote.API"
RUN dotnet build "./ShareNote.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShareNote.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShareNote.API.dll"]