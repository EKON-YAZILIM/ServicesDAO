#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DAO_RFPService/DAO_RFPService.csproj", "DAO_RFPService/"]
RUN dotnet restore "DAO_RFPService/DAO_RFPService.csproj"
COPY . .
WORKDIR "/src/DAO_RFPService"
RUN dotnet build "DAO_RFPService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DAO_RFPService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DAO_RFPService.dll"]