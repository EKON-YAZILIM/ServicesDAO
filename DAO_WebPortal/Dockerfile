#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

#FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
#WORKDIR /app
#

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
RUN apt-get update \ 
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN cd /usr/lib && ln -s libgdiplus.so gdiplus.dll
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DAO_WebPortal/DAO_WebPortal.csproj", "DAO_WebPortal/"]
RUN dotnet restore "DAO_WebPortal/DAO_WebPortal.csproj"
COPY . .
WORKDIR "/src/DAO_WebPortal"
RUN dotnet build "DAO_WebPortal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DAO_WebPortal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DAO_WebPortal.dll"]