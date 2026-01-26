# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Kopiujemy plik projektu z tego samego folderu, w którym jest Dockerfile
COPY *.csproj ./
RUN dotnet restore

# Kopiujemy resztę plików i budujemy
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CarRentalApp.dll"]