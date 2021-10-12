#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DAO_DbService/DAO_DbService.csproj", "DAO_DbService/"]
COPY ["Helpers/Helpers.csproj", "Helpers/"]
RUN dotnet restore "DAO_DbService/DAO_DbService.csproj"
COPY . .
WORKDIR "/src/DAO_DbService"
RUN dotnet build "DAO_DbService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DAO_DbService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DAO_DbService.dll"]
