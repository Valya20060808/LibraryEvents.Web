# Этап сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта (предполагается, что файл .csproj в корне)
COPY *.csproj ./

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем весь исходный код
COPY . ./

# Собираем и публикуем приложение в папку /out в режиме Release
RUN dotnet publish -c Release -o out

# Этап запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копируем опубликованное приложение из этапа сборки
COPY --from=build /app/out ./

# Открываем порт 80 (если используешь другой порт — поменяй)
EXPOSE 80

# Запускаем приложение
ENTRYPOINT ["dotnet", "LibraryEvents.Web.dll"]
