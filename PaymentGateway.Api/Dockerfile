#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY ["PaymentGateway.Library.csproj", "PaymentGateway.Library/"]
RUN dotnet restore "PaymentGateway.Library/PaymentGateway.Library.csproj"

COPY ["PaymentGateway.Api.csproj", "PaymentGateway.Api/"]
RUN dotnet restore "PaymentGateway.Api/PaymentGateway.Api.csproj"

WORKDIR "/src/PaymentGateway.Api"
COPY . .

RUN dotnet build "PaymentGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]