#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["URLS.Web/URLS.Web.csproj", "URLS.Web/"]
RUN dotnet restore "URLS.Web/URLS.Web.csproj"
COPY . .
WORKDIR "/src/URLS.Web"
RUN dotnet build "URLS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLS.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLS.Web.dll"]