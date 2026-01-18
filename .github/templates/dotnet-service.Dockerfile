# Build Stage
ARG SERVICE_NAME
ARG PROJECT_PATH

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy shared props (injected by CI)
COPY Directory.Build.props* ./
COPY global.json* ./

# Copy the service code
COPY . .

# Publish using the provided project path
# e.g. NotificationService/NotificationService.csproj
RUN dotnet publish ${PROJECT_PATH} -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV PORT=8080
EXPOSE 8080

# Note: Railway sets PORT env var. Shell form required for $PORT expansion.
# Railway's startCommand in railway.json overrides this, but keeping as fallback.
CMD dotnet ExecutePlaceholder.dll --urls http://*:${PORT:-8080}
