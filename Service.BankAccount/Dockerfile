FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5075

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service.BankAccount/Service.BankAccount.csproj", "./Service.BankAccount/"]
COPY ["Chassis/Chassis.csproj", "./Chassis/"]
COPY ["Chassis.Grpc/Chassis.Grpc.csproj", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/Shared.Abstractions.csproj", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/Shared.Grpc.csproj", "./Shared.Grpc/"]
RUN dotnet restore "./Service.BankAccount/Service.BankAccount.csproj"

COPY ["Service.BankAccount/", "./Service.BankAccount/"]
COPY ["Chassis/", "./Chassis/"]
COPY ["Chassis.Grpc/", "./Chassis.Grpc/"]
COPY ["Shared.Abstractions/", "./Shared.Abstractions/"]
COPY ["Shared.Grpc/", "./Shared.Grpc/"]
WORKDIR "/src/Service.BankAccount"
RUN dotnet build "Service.BankAccount.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.BankAccount.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.BankAccount.dll"]
