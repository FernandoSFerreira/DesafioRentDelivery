FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DesafioRentDelivery.API/DesafioRentDelivery.API.csproj", "DesafioRentDelivery.API/"]
COPY ["DesafioRentDelivery.Application/DesafioRentDelivery.Application.csproj", "DesafioRentDelivery.Application/"]
COPY ["DesafioRentDelivery.Infrastructure/DesafioRentDelivery.Infrastructure.csproj", "DesafioRentDelivery.Infrastructure/"]
COPY ["DesafioRentDelivery.Domain/DesafioRentDelivery.Domain.csproj", "DesafioRentDelivery.Domain/"]
RUN dotnet restore "DesafioRentDelivery.API/DesafioRentDelivery.API.csproj"
COPY . .
WORKDIR "/src/DesafioRentDelivery.API"
RUN dotnet build "DesafioRentDelivery.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DesafioRentDelivery.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DesafioRentDelivery.API.dll"]
