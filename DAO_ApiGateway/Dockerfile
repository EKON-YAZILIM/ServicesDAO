#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DAO_ApiGateway/DAO_ApiGateway.csproj", "DAO_ApiGateway/"]
COPY ["Helpers/Helpers.csproj", "Helpers/"]
RUN dotnet restore "DAO_ApiGateway/DAO_ApiGateway.csproj"
COPY . .
WORKDIR "/src/DAO_ApiGateway"
RUN dotnet build "DAO_ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DAO_ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DAO_ApiGateway.dll"]
