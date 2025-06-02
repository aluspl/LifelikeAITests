FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ChatGPTChallenge.sln", "./"]
COPY ["Api/Api.csproj", "Api/"]
# Add any other project references here if needed

# Restore dependencies
RUN dotnet restore "ChatGPTChallenge.sln"

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish "Api/Api.csproj" -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]