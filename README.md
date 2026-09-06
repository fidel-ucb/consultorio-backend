# Consultorio Backend

API REST para la gestión de un consultorio psicológico: administración de pacientes, psicólogos y citas, con autenticación y autorización basada en roles.

## Tecnologías

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core** + **SQLite** (persistencia)
- **ASP.NET Core Identity** (gestión de usuarios y roles)
- **JWT (JSON Web Tokens)** (autenticación stateless)

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Herramienta de EF Core CLI (si vas a generar migraciones manualmente):
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

## Puesta en marcha

1. Clonar el repositorio y ubicarte en la carpeta del proyecto:
   ```powershell
   cd consultorio-backend
   ```
2. Restaurar dependencias:
   ```powershell
   dotnet restore
   ```
3. Ejecutar la aplicación:
   ```powershell
   dotnet run
   ```

Al iniciar, la aplicación automáticamente:
- Aplica las migraciones pendientes de EF Core (`Database.Migrate()`), creando `Data/consultorio.db` si no existe.
- Ejecuta el `DatabaseSeeder`, poblando roles, usuarios, psicólogos, pacientes y citas de ejemplo (si la base de datos está vacía).

La API quedará disponible en:
- HTTP: `http://localhost:5110`
- HTTPS: `https://localhost:7263`

La documentación OpenAPI (Swagger) está disponible en modo desarrollo en `/openapi/v1.json`.

## Estructura del proyecto

```
consultorio-backend/
├── Controllers/          # Endpoints de la API, organizados por dominio
│   ├── Auth/              -> Login y registro
│   ├── Patients/           -> CRUD de pacientes
│   ├── Psychologists/      -> Consulta de psicólogos
│   └── Appointments/       -> CRUD de citas
├── DTOs/                 # Contratos de request/response, organizados por dominio
├── Models/               # Entidades de dominio (EF Core)
│   ├── Identity/           -> Constantes de roles (UserRoles)
│   └── Enums/              -> Enumeraciones (AppointmentStatus, AppointmentType)
├── Data/
│   ├── AppDbContext.cs     -> DbContext (IdentityDbContext + entidades de dominio)
│   └── Seeders/            -> Población inicial de datos de prueba
├── Services/Auth/        # Generación de tokens JWT
├── Migrations/           # Historial de migraciones de EF Core
└── Program.cs            # Configuración de servicios y pipeline HTTP
```

## Modelo de dominio

- **`Profile`** (clase base abstracta): datos personales comunes (nombre, DNI, email, teléfono, fecha de nacimiento) y un vínculo opcional a `AppUser` (para quienes tienen acceso a la app).
- **`Patient`** y **`Psychologist`**: heredan de `Profile` (mapeo Table-Per-Type). `Psychologist` añade `LicenceNumber` y `Specialty`.
- **`Appointment`**: relaciona un `Patient` con un `Psychologist`, con fecha/hora de inicio y fin, `Status` (Scheduled, Confirmed, Completed, Cancelled, NoShow) y `Type` (InPerson, Online).
- **`AppUser`**: usuario de autenticación (`IdentityUser<int>`). No todos los pacientes tienen un `AppUser` asociado — algunos solo existen como registro de contacto.

## Autenticación y roles

La autenticación usa el estándar de **ASP.NET Core Identity**: los roles se gestionan mediante las tablas nativas (`AspNetRoles`, `AspNetUserRoles`), usando las constantes definidas en `Models/Identity/UserRoles.cs`:

- `Admin`
- `Psychologist`
- `Patient`

### Flujo de autenticación

1. `POST /api/auth/login` con `email` y `password` devuelve un JWT junto con los roles del usuario.
2. El cliente debe enviar el token en cada request protegido:
   ```
   Authorization: Bearer <token>
   ```
3. Cada endpoint valida el rol requerido mediante `[Authorize(Roles = "...")]`.

### Matriz de permisos (resumen)

| Endpoint | Roles permitidos |
|---|---|
| `POST /api/auth/login`, `POST /api/auth/register` | Público |
| `GET/POST/PUT /api/patients` | Admin, Psychologist |
| `DELETE /api/patients/{id}` | Admin |
| `GET /api/psychologists` | Admin |
| `GET /api/psychologists/me` | Psychologist |
| `GET/POST/PUT/DELETE /api/appointments` | Admin, Psychologist |
| `GET /api/appointments/me` | Patient, Psychologist (filtra según el rol) |

## Datos de prueba (seed)

El `DatabaseSeeder` crea automáticamente los siguientes usuarios (contraseña para todos: `Password@123456`):

| Email | Rol | Notas |
|---|---|---|
| `admin@consultorio.com` | Admin | Usuario administrador del sistema |
| `psic.garcia@consultorio.com` | Psychologist | Con perfil de psicólogo y citas de ejemplo |
| `juan.perez@example.com` | Patient | Con perfil de paciente y varias citas (pasadas y futuras, distintos estados) |

Además se generan psicólogos y pacientes adicionales sin acceso a la app (solo con datos de contacto).

## Configuración

La configuración vive en `appsettings.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=Data/consultorio.db"
  },
  "JwtSettings": {
	"SecretKey": "...",
	"Issuer": "consultorio-backend",
	"Audience": "consultorio-frontend",
	"ExpirationMinutes": 60
  }
}
```

> ⚠️ **Nota de seguridad**: el `SecretKey` de JWT se mantiene en `appsettings.json` para agilizar la entrega de este proyecto. En un entorno de trabajo real debe moverse a **User Secrets**, variables de entorno o un servicio como Azure Key Vault.

## CORS

El backend está configurado para aceptar peticiones desde un frontend en React durante desarrollo:

```csharp
policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
```

Ajusta estos orígenes en `Program.cs` según la URL real de tu frontend.

## Migraciones de base de datos

Para generar una nueva migración tras modificar los modelos:

```powershell
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

Para reiniciar la base de datos desde cero, basta con eliminar el archivo `Data/consultorio.db` (y sus archivos `-wal`/`-shm` si existen); se recreará automáticamente al iniciar la aplicación.
