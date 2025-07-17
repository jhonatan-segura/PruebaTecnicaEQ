### 📦 Proyecto Full Stack (.NET 9 + React + SQL Server)

Este repositorio contiene un sistema de ejemplo con:

- 🖥 Backend: API REST construida con .NET 9
- 🌐 Frontend: Interfaz de usuario hecha con React (Vite + TypeScript)
- 🗄 Base de Datos: Scripts de SQL Server para crear y poblar la base de datos

---
<img width="813" height="624" alt="imagen" src="https://github.com/user-attachments/assets/5f233bdc-15db-45d1-aaa5-a1fed33285a8" />

*Pantalla de inicio creada*

✅ Requisitos

Asegúrate de tener instaladas las siguientes herramientas:

| Herramienta       | Versión Requerida |
|-------------------|-------------------|
| [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) | .NET 9.0 Preview o superior |
| [Node.js](https://nodejs.org/)        | 18.x o superior |
| [NPM](https://www.npmjs.com/)         | 9.x o superior |
| [SQL Server](https://www.microsoft.com/en-us/sql-server/) | Express o superior |
| [Visual Studio Code](https://code.visualstudio.com/) | (opcional) |

---

⚙️ 1. Ejecutar el Backend (DocProcessing.API) .NET 9

📁 Ubicación:
`/backend` (o donde esté ubicado tu proyecto .NET)

Pasos:

1. Abre la terminal en la carpeta del backend.
2. Restaura los paquetes:

   ```bash
   dotnet restore

Ejecuta la API con el comando:

- dotnet run --launch-profile https


Appsettings.json

Configura appsettings.json con tu cadena de conexión a SQL Server.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DocumentProcessing;Trusted_Connection=True;TrustServerCertificate=True"
}
```

El nombre de la base de datos es DocumentProcessing

🛠️ 2. Servicio Worker con .NET (DocProcessing.InspeccionDocumentos)

Este proyecto contiene un **Service Worker** desarrollado con **.NET 9**, diseñado para ejecutar tareas programadas en intervalos definidos desde `appsettings.json`.

---

✅ Requisitos

Antes de ejecutar el proyecto, asegúrate de tener:

| Herramienta       | Versión recomendada |
|-------------------|---------------------|
| [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) | 9.0 o superior |
| Visual Studio o VS Code | Opcional |
| SQL Server o cualquier recurso que consuma el worker | (según lógica del proyecto) |

⚙️ Configuración

El intervalo de ejecución del servicio está definido en el archivo `appsettings.json` mediante una sección llamada `Schedule`. Este valor se utiliza para indicar cada cuántos minutos se debe ejecutar la lógica programada del Worker.

📄 `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Schedule": {
    "IntervalInMinutes": 1
  }
}
```

Puedes modificar IntervalInMinutes para establecer el tiempo entre ejecuciones (en minutos).

🧩 ¿Cómo funciona el Service Worker?

Este proyecto implementa un servicio en segundo plano utilizando BackgroundService o IHostedService. La lógica de procesamiento se ejecuta periódicamente usando el valor configurado.

En Program.cs, el Worker y la configuración del intervalo se registran así:

```csharp
builder.Services.Configure<ScheduleSettings>(
    builder.Configuration.GetSection("Schedule")
);

builder.Services.AddHostedService<Worker>();
```

🚀 Ejecución del Worker

1. Abre una terminal en el directorio raíz del proyecto del Worker

2. Restaura los paquetes necesarios

   ```bash
   dotnet restore

3. Ejecuta el proyecto

   ```bash
   dotnet run


🌐 3. Ejecutar el Frontend (React + Vite)

📁 Ubicación:

Pasos:
1. Abre la terminal en la carpeta del frontend.
2. Instala dependencias:
  npm install
3. Ejecuta el servidor de desarrollo:
  npm run dev
4. Abre en tu navegador:
  http://localhost:5173

🗃️ 4. Ejecutar Scripts de Base de Datos (SQL Server)

📁 Ubicación:

`/scripts`
📝 Contenido esperado:

    setup_database.sql – Crea la base de datos y tablas

    stored_procedures.sql – Crea los procedimientos almacenados

📦 Ejecutar scripts:

Puedes ejecutarlos desde:

- SQL Server Management Studio (SSMS)

- Visual Studio Code con la extensión SQL Server (mssql)

- Terminal (sqlcmd) si está instalado:
   ```bash
   sqlcmd -S localhost -d master -i scripts/01_create_database.sql
   sqlcmd -S localhost -d NombreDB -i scripts/02_seed_data.sql
   ```
   

