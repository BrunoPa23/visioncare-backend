# Etapa de compilacion: restaura y publica con el SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo el csproj para aprovechar la cache de capas en el restore
COPY VisionCareCore/VisionCareCore.csproj VisionCareCore/
RUN dotnet restore VisionCareCore/VisionCareCore.csproj

COPY VisionCareCore/ VisionCareCore/
RUN dotnet publish VisionCareCore/VisionCareCore.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Etapa final: solo el runtime de ASP.NET Core y lo publicado
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
# Usuario sin privilegios que trae la imagen base
USER $APP_UID
ENTRYPOINT ["dotnet", "VisionCareCore.dll"]
