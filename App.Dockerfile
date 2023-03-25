#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GpuTracker.Backend/Server/GpuTracker.Backend.Server.csproj", "GpuTracker.Backend/Server/"]
COPY ["GpuTracker.Database/GpuTracker.Database.csproj", "GpuTracker.Database/"]
COPY ["GpuTracker.Models/GpuTracker.Models.csproj", "GpuTracker.Models/"]
COPY ["GpuTracker.Backend/Client/GpuTracker.Backend.Client.csproj", "GpuTracker.Backend/Client/"]
RUN dotnet restore "GpuTracker.Backend/Server/GpuTracker.Backend.Server.csproj"
COPY . .
WORKDIR "/src/GpuTracker.Backend/Server"
RUN dotnet build "GpuTracker.Backend.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GpuTracker.Backend.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GpuTracker.Backend.Server.dll"]