FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gateway.Root/Gateway.Root.csproj", "./Gateway.Root/"]
COPY ["Chassis/Chassis.csproj", "./Chassis/"]
COPY ["Chassis.Gateway/Chassis.Gateway.csproj", "./Chassis.Gateway/"]
COPY ["Shared.Abstractions/Shared.Abstractions.csproj", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/Shared.Grpc.csproj", "./Shared.Grpc/"]
RUN dotnet restore "./Gateway.Root/Gateway.Root.csproj"

COPY ["Gateway.Root/", "./Gateway.Root/"]
COPY ["Chassis/", "./Chassis/"]
COPY ["Chassis.Gateway/", "./Chassis.Gateway/"]
COPY ["Shared.Abstractions/", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/", "./Shared.Grpc/"]
WORKDIR "/src/Gateway.Root"
RUN dotnet build "Gateway.Root.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.Root.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Root.dll"]
