FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GeneratorDuty/GeneratorDuty.csproj", "GeneratorDuty/"]
COPY ["GeneratorDuty.BuilderHtml/GeneratorDuty.BuilderHtml.csproj", "GeneratorDuty.BuilderHtml/"]
COPY ["GeneratorDuty.Models/GeneratorDuty.Models.csproj", "GeneratorDuty.Models/"]
RUN dotnet restore "GeneratorDuty/GeneratorDuty.csproj"
COPY . .
WORKDIR "/src/GeneratorDuty"
RUN dotnet build "GeneratorDuty.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GeneratorDuty.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeneratorDuty.dll"]
