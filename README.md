# AutoTallerManager

AutoTallerManager es una API RESTful para gestionar las operaciones principales de un taller automotriz: clientes, vehiculos, citas, ordenes de servicio, mecanicos, repuestos, compras, inventario, facturacion, pagos, auditoria y usuarios con roles.

## Arquitectura

El proyecto esta organizado con arquitectura hexagonal:

```text
Api             Adaptador HTTP: controllers, Swagger, JWT, middleware.
Application     Casos de uso, DTOs internos, validaciones, contratos y reglas de aplicacion.
Domain          Entidades, value objects y reglas de dominio.
Infrastructure  EF Core, DbContext, configuraciones, repositorios, Unit of Work y seeders.
docs            Guias de prueba, checklist y scripts auxiliares.
```

## Requisitos

- .NET SDK 10.
- PostgreSQL local o remoto.
- EF Core CLI.
- DBeaver, pgAdmin o cliente SQL equivalente para inspeccionar la base de datos.

Instalar EF Core CLI si no esta disponible:

```bash
dotnet tool install --global dotnet-ef
```

## Configuracion

La cadena de conexion local esta en:

```text
Api/appsettings.Development.json
```

Valor actual de desarrollo:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=AutoTallerManagerDb;Username=postgres;Password=1234;Include Error Detail=true"
  }
}
```

Para entrega o produccion, reemplazar la cadena de conexion y la clave JWT por valores del entorno. No usar la clave JWT de desarrollo en produccion.

## Base de datos

Aplicar migraciones:

```bash
dotnet ef database update -p .\Infrastructure -s .\Api
```

Verificar si hay cambios pendientes de modelo:

```bash
dotnet ef migrations has-pending-model-changes -p .\Infrastructure -s .\Api
```

Si PostgreSQL devuelve errores de llave primaria duplicada por secuencias desfasadas, ejecutar el script:

```text
docs/sql/sincronizar-secuencias-postgres.sql
```

## Ejecucion

Levantar la API:

```bash
dotnet run --project .\Api
```

Swagger queda disponible en:

```text
http://localhost:5258/swagger
```

## Datos semilla

Al iniciar la API, el sistema crea datos base si no existen:

- Roles: `Admin`, `Mecanico`, `Recepcionista`.
- Estados de orden: `Pendiente`, `En proceso`, `Completada`, `Cancelada`.
- Estados de factura: `Emitida`, `Pagada`, `Anulada`.
- Metodos de pago: `Efectivo`, `Tarjeta`, `Transferencia`.
- Tipos de servicio: `Diagnostico`, `Mantenimiento preventivo`, `Reparacion`.
- Categorias de repuesto: `Encendido`, `Frenos`, `Filtros`.
- Unidades de medida: `Unidad`, `Litro`.

En ambiente `Development`, tambien crea usuarios de prueba:

```json
{
  "correo": "admin.seed@autotaller.com",
  "contrasena": "Admin123!"
}
```

```json
{
  "correo": "recepcionista.seed@autotaller.com",
  "contrasena": "Recepcionista123!"
}
```

```json
{
  "correo": "mecanico.seed@autotaller.com",
  "contrasena": "Mecanico123!"
}
```

## Autenticacion

Login:

```http
POST /api/Auth/login
```

Body:

```json
{
  "correo": "admin.seed@autotaller.com",
  "contrasena": "Admin123!"
}
```

En Swagger, usar el boton `Authorize` y pegar el token JWT devuelto por el login.

## Roles

- `Admin`: acceso administrativo, catalogos, usuarios, inventario y configuracion.
- `Recepcionista`: clientes, vehiculos, citas y ordenes de servicio.
- `Mecanico`: asignaciones, tareas, notas, detalles de orden, facturas, pagos y garantias.

## Modulos principales

- `Cliente`
- `Vehiculo`
- `Cita`
- `OrdenServicio`
- `OrdenMecanico`
- `OrdenTipoServicio`
- `TareaMecanico`
- `DetalleOrden`
- `Repuesto`
- `Compra`
- `DetalleCompra`
- `Factura`
- `Pago`
- `Auditoria`
- `LogInventario`
- Catalogos: roles, estados, metodos de pago, ubicaciones, marcas, modelos, categorias y unidades.

## Reglas de negocio destacadas

- Un vehiculo no puede tener dos ordenes activas.
- Un mecanico no puede estar asignado a dos ordenes activas.
- No se puede crear una cita para un vehiculo con orden activa.
- No se permiten citas cruzadas para el mismo vehiculo.
- No se puede modificar una orden completada o cancelada.
- No se puede facturar una orden cancelada.
- Los detalles de compra aumentan stock.
- Los detalles de orden descuentan stock.
- Los movimientos de inventario se registran en `LogInventario`.
- Las acciones importantes se registran en `Auditoria`.
- No se puede modificar o eliminar un pago confirmado.
- No se puede modificar una factura con pagos confirmados.

## Paginacion y filtros

Listados principales:

```http
GET /api/Cliente?pageNumber=1&pageSize=20&search=Carlos
GET /api/Vehiculo?pageNumber=1&pageSize=20&clienteId=2
GET /api/OrdenServicio?pageNumber=1&pageSize=20&estadoId=1
GET /api/Repuesto?pageNumber=1&pageSize=20&soloBajoStock=true
```

Los listados paginados devuelven el header:

```text
X-Total-Count
```

## Rate limiting

- `OrdenServicio`: 60 solicitudes por minuto.
- `Repuesto`: 30 solicitudes por minuto.

Al exceder el limite, la API responde:

```text
429 Too Many Requests
```

## Comandos utiles

Compilar:

```bash
dotnet build --no-restore
```

Aplicar migraciones:

```bash
dotnet ef database update -p .\Infrastructure -s .\Api
```

Verificar cambios pendientes:

```bash
dotnet ef migrations has-pending-model-changes -p .\Infrastructure -s .\Api
```

Ejecutar API:

```bash
dotnet run --project .\Api
```

## Documentacion adicional

- `docs/checklist-entrega.md`
- `docs/flujo-pruebas-swagger.md`
- `docs/revision-api-swagger.md`
- `docs/estado-modulos-entidades.md`
- `docs/sql/sincronizar-secuencias-postgres.sql`
