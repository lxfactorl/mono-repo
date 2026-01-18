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

# Note: Entrypoint is set by Railway via startCommand in railway.json 
# or we can use a generic launcher. For consistency with our current setup:
ENTRYPOINT ["dotnet", "ExecutePlaceholder.dll", "--urls", "http://*:$PORT"]
