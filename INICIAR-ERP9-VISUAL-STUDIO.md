# Dejar ERP9 listo en una PC con Visual Studio y SQL Server

Estos pasos son para Windows, Visual Studio y SQL Server instalados en la PC. La migración `InitialCreate` ya está incluida: no hay que crear otra.

## 1. Abrir el proyecto

Extraiga el ZIP completo en una carpeta, por ejemplo `C:\Proyectos\ERP9`. Abra `ERP.slnx` en una versión de Visual Studio compatible con .NET 10 y espere a que termine la restauración de paquetes NuGet. Debe tener instalada la carga de trabajo **Desarrollo de ASP.NET y web**; también puede instalar el SDK .NET 10 por separado.

En Visual Studio, seleccione `ERP.WebApi` como proyecto de inicio.

## 2. Configurar SQL Server y el secreto local

Abra **PowerShell** en la carpeta donde está `ERP.slnx`. El ejemplo recomendado usa autenticación de Windows, así que la cuenta de Windows debe tener permiso para crear la base `ERP9` (o cree la base vacía en SQL Server Management Studio y dé permisos al usuario).

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ERP9;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False" --project ERP.WebApi
dotnet user-secrets set "Seed:AdminPassword" "CAMBIE_ESTA_CLAVE_LOCAL" --project ERP.WebApi
dotnet restore ERP.slnx
dotnet ef database update --project ERP.Infraestructure --startup-project ERP.WebApi
```

Reemplace `CAMBIE_ESTA_CLAVE_LOCAL` por una clave privada de desarrollo. User Secrets la guarda fuera del proyecto; no se sube al repositorio. Si `dotnet-ef` ya está instalado, el primer comando puede responder que ya existe; en ese caso ejecute `dotnet tool update --global dotnet-ef --version 10.0.12`.

Si SQL Server usa una instancia nombrada, cambie el servidor en la cadena, por ejemplo `Server=localhost\SQLEXPRESS;...`. Si usan autenticación de SQL en vez de Windows, use esta forma y reemplace usuario y clave por los de su servidor:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ERP9;User ID=SU_USUARIO;Password=SU_CLAVE;TrustServerCertificate=True;Encrypt=False" --project ERP.WebApi
```

Después de cambiar la conexión, vuelva a ejecutar `dotnet ef database update --project ERP.Infraestructure --startup-project ERP.WebApi`.

## 3. Ejecutar y comprobar

Presione **F5** en Visual Studio. El perfil `http` inicia la API en `http://localhost:5239` y abre Scalar; también puede ir a `http://localhost:5239/scalar` manualmente. Use Scalar para probar los casos del laboratorio. En el perfil Development se cargan los datos de demostración del seeder.

Si la consola informa que no puede conectar con SQL Server, confirme que el servicio SQL Server está iniciado, que el nombre de instancia en la cadena es correcto y que la cuenta tiene acceso. No hace falta ejecutar `migrations add`: esa migración ya está incluida.

## Para otra PC

Copie el repositorio o el ZIP extraído. En cada PC repita los pasos 2 y 3 con su propio SQL Server y sus propios User Secrets. No copie ni comparta `appsettings.Development.json` con contraseñas; la configuración local va en User Secrets.
