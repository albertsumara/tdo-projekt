FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CarRentalApp/CarRentalApp.csproj", "CarRentalApp/"]
RUN dotnet restore "CarRentalApp/CarRentalApp.csproj"
COPY . .
WORKDIR "/src/CarRentalApp"
RUN dotnet build "CarRentalApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarRentalApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarRentalApp.dll"]