#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
RUN apt-get update -y
RUN apt-get install -y libsnappy-dev
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY GpuTracker.DatabaseConsumer/GpuTracker.DatabaseConsumer.csproj GpuTracker.DatabaseConsumer/
COPY GpuTracker.Common/GpuTracker.Common.csproj GpuTracker.Common/
COPY GpuTracker.Models/GpuTracker.Models.csproj GpuTracker.Models/
COPY GpuTracker.Database/GpuTracker.Database.csproj GpuTracker.Database/
RUN dotnet restore GpuTracker.DatabaseConsumer/GpuTracker.DatabaseConsumer.csproj
COPY . .
WORKDIR "/src/GpuTracker.DatabaseConsumer"
RUN dotnet build "GpuTracker.DatabaseConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GpuTracker.DatabaseConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GpuTracker.DatabaseConsumer.dll"]