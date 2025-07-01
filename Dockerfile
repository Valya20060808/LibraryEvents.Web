FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем решение и проект в рабочую папку
COPY LibraryEvents.Web.sln .
COPY LibraryEvents.Web.csproj .

# Восстанавливаем зависимости (nuget)
RUN dotnet restore

# Копируем остальные исходники проекта
COPY . .

# Публикуем проект, явно указывая путь к csproj
RUN dotnet publish LibraryEvents.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "LibraryEvents.Web.dll"]
