FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
RUN apt-get update -y
RUN apt-get install -y libsnappy-dev
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GpuTracker.Producer/GpuTracker.Producer.csproj", "GpuTracker.Producer/"]
COPY ["GpuTracker.Common/GpuTracker.Common.csproj", "GpuTracker.Common/"]
COPY ["GpuTracker.Models/GpuTracker.Models.csproj", "GpuTracker.Models/"]
RUN dotnet restore "GpuTracker.Producer/GpuTracker.Producer.csproj"
COPY . .
WORKDIR "/src/GpuTracker.Producer"
RUN dotnet build "GpuTracker.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GpuTracker.Producer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GpuTracker.Producer.dll"]