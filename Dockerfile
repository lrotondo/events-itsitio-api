FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["events-api/events-api.csproj", "events-api/"]
RUN dotnet restore "events-api/events-api.csproj"
COPY . .
WORKDIR "/src/events-api"
RUN dotnet build "events-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "events-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet events-api.dll