FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081

ENV ASPNETCORE_URLS=http://*:8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["analytical_management/analytical_management.csproj", "analytical_management/"]
RUN dotnet restore "analytical_management/analytical_management.csproj"
COPY . .
WORKDIR "/src/analytical_management"
RUN dotnet build "analytical_management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "analytical_management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "analytical_management.dll"]
