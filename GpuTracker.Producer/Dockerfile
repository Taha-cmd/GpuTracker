FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
RUN apt-get update -y
RUN apt-get install -y libsnappy-dev

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY **.csproj .
RUN dotnet restore .
COPY . .
RUN dotnet build -c Release GpuTracker.Producer.csproj -o /build

FROM runtime AS final
WORKDIR /app
COPY --from=build /build .
CMD ["./GpuTracker.Producer"]
