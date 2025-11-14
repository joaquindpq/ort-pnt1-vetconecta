# 🐾 AppMascotas – Guía de Instalación

Este proyecto utiliza **.NET 8**, **Entity Framework Core** y **SQL Server**.  
(Las rutas de navegación que se mencionan deben reemplazarse por las configuradas en cada computadora)

## 📥 1. Clonar el proyecto directamente desde Visual Studio 2022

1. Abrir **Visual Studio 2022**.
2. En la pantalla inicial, hacer clic en **Clone a repository**.
3. En el campo **Repository location**, ingresar la URL del repositorio: https://github.com/joaquindpq/ort-pnt1-vetconecta.git
4. Elegir la carpeta donde querés descargar el proyecto.
5. Hacer clic en **Clone**.

## 📌 2. Importar el proyecto en Visual Studio

1. Abrir **Visual Studio 2022**.  
2. Ir a **File > Open > Project/Solution**.  
3. Navegar hasta: C:\Users\Tiendamia1\source\repos\AppMascotas\AppMascotas.sln
4. Seleccionar el archivo `.sln`.



## 📦 3. Restaurar los paquetes NuGet

1. En Visual Studio, ir a:  
   **Tools > NuGet Package Manager > Package Manager Console**
2. Ejecutar: **dotnet restore**



## 🗄️ 3. Configurar la base de datos (opcional)

Para evitar conflictos con Entity Framework al iniciar por primera vez:

1. Abrir la consola NuGet:  
   **Tools > NuGet Package Manager > Package Manager Console**
2. Ejecutar: **Update-Database**
Esto aplicará la migración existente y creará las tablas en **SQL Server**.

> **Nota:** La migración solo crea la estructura de la base de datos. No configuramos un seed de datos de prueba por defecto.


## ▶️ 4. Ejecutar el proyecto

1. Presionar **F5** o ir a:  
   **Debug > Start Debugging**
2. Transcurridos unos segundos, la aplicación se abrirá en el navegador en la ruta: https://localhost:7218/

