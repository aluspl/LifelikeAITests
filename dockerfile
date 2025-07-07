FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AiAgent.sln", "./"]
COPY ["AiAgent.Api/AiAgent.Api.csproj", "AiAgent.Api/"]
COPY ["AiAgent.Api.Tests/AiAgent.Api.Tests.csproj", "AiAgent.Api.Tests/"]
# Add any other project references here if needed

# Restore dependencies
RUN dotnet restore "AiAgent.sln"

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish "AiAgent.Api/AiAgent.Api.csproj" -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AiAgent.Api.dll"]