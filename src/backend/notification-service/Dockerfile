# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the shared properties and global config if they exist in the build context
# Note: These will be copied from the local directory by the CLI
COPY Directory.Build.props* ./
COPY global.json* ./

# Copy the service code
COPY . .

# Publish the service
RUN dotnet publish NotificationService/NotificationService.csproj -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Railway provides the PORT environment variable
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.dll", "--urls", "http://*:$PORT"]
