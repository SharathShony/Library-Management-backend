# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY ["Libraray.Api.csproj", "./"]
RUN dotnet restore "Libraray.Api.csproj"

# Copy all source files
COPY . .

# Build the project
RUN dotnet build "Libraray.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Libraray.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Render uses dynamic PORT - expose it but it will be overridden
EXPOSE 8080
ENV PORT=8080

# Set ASP.NET Core to listen on PORT variable (important for Render)
ENV ASPNETCORE_URLS=http://+:${PORT}

# Copy published output
COPY --from=publish /app/publish .

# Set entrypoint
ENTRYPOINT ["dotnet", "Libraray.Api.dll"]
