FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AiAgent.sln", "./"]
COPY ["AiAgent.Api/AiAgent.Api.csproj", "AiAgent.Api/"]
COPY ["AiAgent.Api.Tests/AiAgent.Api.Tests.csproj", "AiAgent.Api.Tests/"]

# Copy everything else
COPY . .

# Restore dependencies
RUN dotnet restore "AiAgent.sln"

# Build and publish
FROM build AS publish
RUN dotnet publish "AiAgent.Api/AiAgent.Api.csproj" -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app .
COPY AiAgent.Api/Domain/Instructions/ /app/Domain/Instructions/
COPY Assets/ /app/Assets/
EXPOSE 8080
ENTRYPOINT ["dotnet", "AiAgent.Api.dll"]