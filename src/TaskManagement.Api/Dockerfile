FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TaskManagement.Domain/TaskManagement.Domain.csproj", "TaskManagement.Domain/"]
RUN dotnet restore "TaskManagement.Domain/TaskManagement.Domain.csproj"
COPY ["src/TaskManagement.Persistence/TaskManagement.Persistence.csproj", "TaskManagement.Persistence/"]
RUN dotnet restore "TaskManagement.Persistence/TaskManagement.Persistence.csproj"
COPY ["src/TaskManagement.Service/TaskManagement.Service.csproj", "TaskManagement.Service/"]
RUN dotnet restore "TaskManagement.Service/TaskManagement.Service.csproj"
COPY ["src/TaskManagement.Api/TaskManagement.Api.csproj", "TaskManagement.Api/"]
RUN dotnet restore "TaskManagement.Api/TaskManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/TaskManagement.Api"
RUN dotnet build "TaskManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaskManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]