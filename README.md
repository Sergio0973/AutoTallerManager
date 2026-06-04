# AutoTallerManager

AutoTallerManager es una solucion para gestionar las operaciones de un taller automotriz. Incluye API RESTful en ASP.NET Core, persistencia con Entity Framework Core y PostgreSQL, autenticacion JWT, roles, auditoria, control de inventario, facturacion y un frontend en Next.js conectado al backend.

## Arquitectura

El backend sigue arquitectura hexagonal:

```text
Api             Adaptador HTTP: controllers, Swagger, JWT, middleware y configuracion web.
Application     Casos de uso, validaciones, contratos, reglas de aplicacion y abstracciones.
Domain          Entidades, value objects, enums y reglas del negocio.
Infrastructure  EF Core, DbContext, configuraciones Fluent API, repositorios, Unit of Work y seeders.
frontend        Aplicacion Next.js/React conectada a la API.
docs            Guias de prueba, checklist y scripts auxiliares.
```

## Requisitos

- .NET SDK 10.
- PostgreSQL.
- Node.js compatible con Next.js 16.
- pnpm.
- EF Core CLI.
- DBeaver, pgAdmin o cliente SQL equivalente.

Instalar EF Core CLI si no esta disponible:

```bash
dotnet tool install --global dotnet-ef
```

Instalar pnpm si no esta disponible:

```bash
npm install -g pnpm
```

## Configuracion

La configuracion local del backend esta en:

```text
Api/appsettings.Development.json
```

Cadena de conexion usada en desarrollo:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=AutoTallerManagerDb;Username=postgres;Password=1234;Include Error Detail=true"
  }
}
```

JWT de desarrollo:

```json
{
  "Jwt": {
    "Issuer": "AutoTallerManager",
    "Audience": "AutoTallerManager",
    "Key": "AutoTallerManager-Development-Key-Change-Me-For-Production-2026",
    "ExpirationMinutes": 120
  }
}
```

Para entrega o produccion, cambiar la cadena de conexion y la clave JWT por valores seguros del entorno. No usar la clave JWT de desarrollo en produccion.

## Base de datos

Aplicar migraciones:

```bash
dotnet ef database update -p .\Infrastructure -s .\Api
```

Verificar si hay cambios pendientes de modelo:

```bash
dotnet ef migrations has-pending-model-changes -p .\Infrastructure -s .\Api
```

Si PostgreSQL devuelve errores por secuencias desfasadas, ejecutar en la base de datos:

```text
docs/sql/sincronizar-secuencias-postgres.sql
```

## Ejecucion del backend

Desde la raiz del proyecto:

```bash
dotnet run --project .\Api
```

La API queda disponible en:

```text
http://localhost:5258
```

Swagger queda disponible en:

```text
http://localhost:5258/swagger
```

## Ejecucion del frontend

Desde la carpeta `frontend`:

```bash
pnpm install
pnpm dev
```

El frontend queda disponible en:

```text
http://localhost:3000
```

El cliente HTTP del frontend usa por defecto:

```text
/backend-api
```

Si se necesita apuntar directamente a otro backend, configurar:

```text
NEXT_PUBLIC_API_URL=http://localhost:5258/api
```

## Datos semilla

Al iniciar la API, el sistema crea datos base si no existen:

- Roles: `Admin`, `Mecanico`, `Recepcionista`.
- Estados de orden: `RECIBIDA`, `DIAGNOSTICO`, `REPARACION`, `LISTA`, `ENTREGADA`, `CANCELADA`.
- Estados de factura: `Emitida`, `Pagada`, `Anulada`.
- Metodos de pago: `Efectivo`, `Tarjeta`, `Transferencia`.
- Tipos de servicio: `CAMBIO_DE_ACEITE`, `MANTENIMIENTO_PREVENTIVO`, `DIAGNOSTICO`, `REPARACION`.
- Inventario base, categorias y unidades de medida.

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

En Swagger, usar `Authorize` y pegar el token JWT con formato:

```text
Bearer {token}
```

En el frontend, el login guarda el token y datos del usuario en cookies.

## Roles y permisos

- `Admin`: acceso total a usuarios, roles, catalogos, inventario, proveedores, compras, facturacion, auditoria y configuracion.
- `Recepcionista`: clientes, vehiculos, citas y ordenes de servicio.
- `Mecanico`: ordenes asignadas, tareas mecanicas, repuestos usados y facturacion permitida.

El menu lateral del frontend se adapta segun el rol autenticado.

## Modulos del backend

- Auth
- Usuario
- Rol
- Cliente
- ClienteCorreo
- ClienteTelefono
- ClienteDireccion
- Vehiculo
- Marca
- ModeloVehiculo
- Cita
- OrdenServicio
- OrdenTipoServicio
- OrdenMecanico
- TareaMecanico
- NotaOrden
- HistorialEstadoOrden
- DetalleOrden
- Repuesto
- CategoriaRepuesto
- UnidadMedida
- Proveedor
- RepuestoProveedor
- Compra
- DetalleCompra
- LogInventario
- Factura
- EstadoFactura
- MetodoPago
- Pago
- Auditoria
- Pais
- Departamento
- Ciudad

## Modulos del frontend

- Login y proteccion de rutas.
- Dashboard.
- Clientes con correo, telefono y direccion.
- Vehiculos con marcas y modelos.
- Citas.
- Ordenes de servicio.
- Detalle operativo de orden para mecanicos.
- Inventario.
- Proveedores, compras y detalles de compra.
- Facturacion y pagos.
- Auditoria.
- Usuarios y roles.
- Perfil.
- Configuracion y catalogos de vehiculos.

## Reglas de negocio principales

- Un vehiculo no puede tener dos ordenes activas.
- Un vehiculo con orden activa no puede agendar nueva cita.
- Un mecanico no puede tener dos ordenes activas.
- No se puede asignar mecanico a orden terminal.
- No se puede facturar una orden cancelada.
- No se puede modificar una orden completada o cancelada.
- Los detalles de compra aumentan stock.
- Los detalles de orden descuentan stock.
- Los movimientos de inventario se registran en `LogInventario`.
- Las acciones importantes se registran en `Auditoria`.
- No se puede modificar o eliminar un pago confirmado.
- No se puede modificar una factura con pagos confirmados.
- No se pueden eliminar registros con dependencias operativas.

## Paginacion y filtros

Ejemplos:

```http
GET /api/Cliente?pageNumber=1&pageSize=20&search=Carlos
GET /api/Vehiculo?pageNumber=1&pageSize=20&clienteId=2
GET /api/OrdenServicio?pageNumber=1&pageSize=20&estadoId=1
GET /api/Repuesto?pageNumber=1&pageSize=20&soloBajoStock=true
GET /api/Auditoria?entidad=OrdenServicio
GET /api/Pago?facturaId=7
```

Los listados paginados devuelven:

```text
X-Total-Count
```

## Rate limiting

- `OrdenServicio`: 60 solicitudes por minuto.
- `Repuesto`: 30 solicitudes por minuto.

Al exceder el limite:

```text
429 Too Many Requests
```

## Flujo de prueba recomendado

1. Iniciar sesion como `Admin`.
2. Crear o verificar roles y usuarios.
3. Crear cliente con correo, telefono y direccion.
4. Crear marca/modelo si hace falta.
5. Crear vehiculo para el cliente.
6. Crear cita como recepcionista.
7. Crear orden de servicio vinculada a la cita.
8. Cambiar estado de la orden a diagnostico.
9. Asignar mecanico.
10. Iniciar sesion como mecanico.
11. Registrar trabajo realizado.
12. Agregar repuesto usado.
13. Verificar descuento de stock en inventario.
14. Iniciar sesion como Admin o rol permitido.
15. Generar factura.
16. Registrar pago.
17. Verificar auditoria y log de inventario.

## Validaciones antes de entregar

Backend:

```bash
dotnet build --no-restore
```

Frontend:

```bash
cd frontend
pnpm exec tsc --noEmit
pnpm build
```

Migraciones:

```bash
dotnet ef database update -p .\Infrastructure -s .\Api
```

Nota: el script `pnpm lint` existe, pero requiere que `eslint` este instalado/configurado en el frontend.

## Documentacion adicional

- `docs/checklist-entrega.md`
- `docs/flujo-pruebas-swagger.md`
- `docs/revision-api-swagger.md`
- `docs/estado-modulos-entidades.md`
- `docs/prueba-final-flujo.md`
- `docs/sql/sincronizar-secuencias-postgres.sql`

## Estado general

El proyecto cuenta con backend funcional, frontend conectado, autenticacion JWT, control por roles, auditoria, inventario, compras, facturacion, pagos y flujo operativo completo probado desde Swagger y frontend.
