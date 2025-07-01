# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ImageService.Domain/ImageService.Domain.csproj", "ImageService.Domain/"]
COPY ["ImageService.Application/ImageService.Application.csproj", "ImageService.Application/"]
COPY ["ImageService.Infrastructure/ImageService.Infrastructure.csproj", "ImageService.Infrastructure/"]
COPY ["ImageService.API/ImageService.API.csproj", "ImageService.API/"]
RUN dotnet restore "ImageService.API/ImageService.API.csproj"
COPY . .
RUN dotnet publish "ImageService.API/ImageService.API.csproj" -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS="https://+:8083;http://+:8082" \
    ASPNETCORE_HTTPS_PORT=8083 \
    ASPNETCORE_HTTP_PORTS=8082
EXPOSE 8082 8083
ENTRYPOINT ["dotnet", "ImageService.API.dll"]