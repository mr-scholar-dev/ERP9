# Laboratorio 9: Validaciones

## Preparar la base ERP9

Los pasos para ejecutar desde Visual Studio con SQL Server en Windows están en [INICIAR-ERP9-VISUAL-STUDIO.md](INICIAR-ERP9-VISUAL-STUDIO.md). Resumen: configure `ConnectionStrings:DefaultConnection` y `Seed:AdminPassword` en User Secrets, ejecute `dotnet ef database update --project ERP.Infraestructure --startup-project ERP.WebApi` una sola vez y luego inicie `ERP.WebApi` con F5.

La migración inicial ya está incluida; no ejecute `migrations add`. La aplicación publica Scalar en Development. Use allí los casos de prueba del PDF, en especial las validaciones de DTO, el rango de precio, las categorías inexistentes y los nombres/cédulas duplicados.

## Ejercicio: modelo o regla de negocio

| Validación | Tipo y ubicación | HTTP | Justificación |
| --- | --- | ---: | --- |
| Precio mayor que cero | Modelo, DTO de producto | 400 | El rango se determina con el valor recibido. |
| Inventario mayor o igual a cero | Modelo, DTO de producto | 400 | No requiere consultar el estado del sistema. |
| Nombre del producto obligatorio | Modelo, DTO de producto | 400 | `Required` comprueba presencia y contenido. |
| Extensión de imagen permitida | Modelo, atributo propio en DTO | 400 | Se valida inspeccionando el nombre del archivo. |
| Precio mínimo ≤ precio máximo | Modelo, `IValidatableObject` de búsqueda | 400 | Compara dos propiedades de la misma solicitud. |
| Categoría existente | Regla de negocio, Service | 400 | El repositorio consulta la base para comprobarla. |
| Bodega existente | Regla de negocio, Service | 400 | El repositorio consulta la base para comprobarla. |
| Proveedor activo | Regla de negocio, Service | 400 | El estado se obtiene de la base de datos. |
| Cédula jurídica con formato 3-101-123456 | Modelo, DTO de proveedor | 400 | La expresión regular valida el formato sin consultar la base. |
| Cédula jurídica única | Regla de negocio, Service | 409 | La duplicidad depende de otros registros existentes. |
| Correo del cliente con formato válido | Modelo, DTO de cliente | 400 | El formato se valida con el dato de la solicitud. |
| Login de usuario único | Regla de negocio, Service | 409 | La unicidad requiere consultar otros usuarios. |
| No eliminar un proveedor con productos | Regla de negocio, Service | 409 | La relación se determina consultando la base. |

## Reto A: Warehouse

Los DTOs de creación y edición tienen mensajes de validación en español. `ExistsByNameAsync` excluye el ID actual al editar; el Service devuelve 409 para otro registro con el mismo nombre y permite actualizar la bodega conservando su propio nombre.
