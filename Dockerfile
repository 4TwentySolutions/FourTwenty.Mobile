FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY XamBasePacket.sln ./
COPY XamBasePacket/*.csproj ./XamBasePacket/
COPY XamBasePacket.Tests/*.csproj ./XamBasePacket.Tests/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/XamBasePacket
RUN dotnet build -c Release

# Publishing
RUN dotnet publish -c Release -o /app/out

# Run tests
WORKDIR /app/XamBasePacket.Tests
RUN dotnet test

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app /app/out
ENTRYPOINT ["dotnet", "XamBasePacket.dll"]
