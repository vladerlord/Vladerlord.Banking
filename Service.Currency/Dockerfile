FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5077

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service.Currency/Service.Currency.csproj", "./Service.Currency/"]
COPY ["Chassis/Chassis.csproj", "./Chassis/"]
COPY ["Chassis.Grpc/Chassis.Grpc.csproj", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/Shared.Abstractions.csproj", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/Shared.Grpc.csproj", "./Shared.Grpc/"]
RUN dotnet restore "./Service.Currency/Service.Currency.csproj"

COPY ["Service.Currency/", "./Service.Currency/"]
COPY ["Chassis/", "./Chassis/"]
COPY ["Chassis.Grpc/", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/", "./Shared.Grpc/"]
WORKDIR "/src/Service.Currency"
RUN dotnet build "Service.Currency.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.Currency.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Currency.dll"]
