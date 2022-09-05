FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service.IdentityNotifier/Service.IdentityNotifier.csproj", "./Service.IdentityNotifier/"]
COPY ["Chassis/Chassis.csproj", "./Chassis/"]
COPY ["Chassis.Grpc/Chassis.Grpc.csproj", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/Shared.Abstractions.csproj", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/Shared.Grpc.csproj", "./Shared.Grpc/"]
COPY ["Service.IdentityNotifier.RazorTemplates/Service.IdentityNotifier.RazorTemplates.csproj", "./Service.IdentityNotifier.RazorTemplates/"]
RUN dotnet restore "./Service.IdentityNotifier/Service.IdentityNotifier.csproj"

COPY ["Service.IdentityNotifier/", "./Service.IdentityNotifier/"]
COPY ["Chassis/", "./Chassis/"]
COPY ["Chassis.Grpc/", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/", "./Shared.Grpc/"]
COPY ["Service.IdentityNotifier.RazorTemplates/", "./Service.IdentityNotifier.RazorTemplates/"]
WORKDIR "/src/Service.IdentityNotifier"
RUN dotnet build "Service.IdentityNotifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.IdentityNotifier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.IdentityNotifier.dll"]
