FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5074
EXPOSE 5500

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service.Identity/Service.Identity.csproj", "./Service.Identity/"]
COPY ["Chassis/Chassis.csproj", "./Chassis/"]
COPY ["Chassis.Grpc/Chassis.Grpc.csproj", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/Shared.Abstractions.csproj", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/Shared.Grpc.csproj", "./Shared.Grpc/"]
RUN dotnet restore "./Service.Identity/Service.Identity.csproj"

COPY ["Service.Identity/", "./Service.Identity/"]
COPY ["Chassis/", "./Chassis/"]
COPY ["Chassis.Grpc/", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/", "./Shared.Grpc/"]
WORKDIR "/src/Service.Identity"
RUN dotnet build "Service.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Identity.dll"]
