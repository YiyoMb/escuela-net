# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el .csproj correcto
COPY ["SistemaEscolar.API.csproj", "./"]
RUN dotnet restore "SistemaEscolar.API.csproj"

COPY . .
RUN dotnet build "SistemaEscolar.API.csproj" -c Release -o /app/build
RUN dotnet publish "SistemaEscolar.API.csproj" -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SistemaEscolar.API.dll"]