<p align="center">
<image src="img/logo.png" />
</p>

# Немного о 🚖
<b>GetMeThere (или GetMe или Забери)</b> - пет-проект для вызова такси.

# Стек
- **API**: ASP.NET Core (.NET 9), EF Core 9, MySQL (Pomelo), SignalR, Serilog, JWT.
- **Mobile**: .NET MAUI (.NET 9) — `Mobile.Shared` (Refit, SignalR.Client) + `Mobile.Rider` (пассажир) + `Mobile.Driver` (водитель), Mapsui для карт OSM.

# Запуск API локально

## Pre-requirements
- .NET 9 SDK.
- MySQL 8 (`server=localhost;user=root;password=...;database=taxidb` либо переопределить через `appsettings.Development.json` / env var).

## Настройка секретов
JWT-ключ хранится только вне репозитория. Сгенерируйте сильный ключ (≥32 символа) и положите в user-secrets:

```bash
cd src/Services/GetMeThere.API
dotnet user-secrets set "Jwt:Key" "<your-32+-char-random-key>"
```

Альтернатива — переменная окружения `Jwt__Key`. Без ключа API не стартует.

## Применение миграции
SQL-скрипт лежит в `migrations/Mvp_AuthAndOrderState.sql` (идемпотентный). Применить:

```bash
mysql -u root -p taxidb < migrations/Mvp_AuthAndOrderState.sql
```

⚠️ После переименования `User.Password → PasswordHash` старые байты пароля **не пройдут** bcrypt-verify — старые аккаунты пересоздать.

## Запуск
```bash
cd src/Services/GetMeThere.API
dotnet run
# Swagger: http://localhost:5000/swagger
# Health: GET /health
# SignalR: /taxiHub (?access_token=<jwt> для авторизации)
```

# Запуск Mobile

## Pre-requirements
- .NET 9 SDK + MAUI workloads (`android`, `ios`, `maccatalyst`, `maui-windows`).
- Для Android-эмулятора base URL API — `http://10.0.2.2:5000` (см. `Mobile.Shared/Configuration/AppConfig.cs`).

## Сборка
```bash
cd src/Mobile
dotnet build Mobile.sln -f net9.0-android       # все три проекта
dotnet build -t:Run -f net9.0-android Mobile.Rider/Mobile.Rider.csproj
dotnet build -t:Run -f net9.0-android Mobile.Driver/Mobile.Driver.csproj
```
