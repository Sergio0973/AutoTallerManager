# Estado de modulos por entidad

Este documento compara las entidades de `Domain/Entities` contra las piezas que deberia tener cada modulo en la arquitectura actual del proyecto.

Convenciones actuales:

- `Domain/Entities`: entidad de dominio.
- `Infrastructure/Context/AutoTallerDbContext.cs`: `DbSet`.
- `Infrastructure/Configuration`: configuracion EF Core con Fluent API.
- `Application/Abstractions`: interfaz de repositorio.
- `Infrastructure/Repositories`: implementacion del repositorio.
- `Application/<Modulo>/UseCase`: casos de uso con MediatR y FluentValidation.
- `Api/<Modulo>`: contracts y controlador REST.

## Matriz actual

| Entidad | DbSet | Configuration | Repository | UseCases | API |
|---|---:|---:|---:|---:|---:|
| Auditoria | Si | Si | No | No | No |
| CategoriaRepuesto | Si | Si | No | No | No |
| Cita | Si | Si | No | No | No |
| Ciudad | Si | Si | No | No | No |
| Cliente | Si | Si | Si | Si | Si |
| ClienteCorreo | Si | Si | No | No | No |
| ClienteDireccion | Si | Si | No | No | No |
| ClienteTelefono | Si | Si | No | No | No |
| Compra | Si | Si | No | No | No |
| Departamento | Si | Si | No | No | No |
| DetalleCompra | Si | Si | No | No | No |
| DetalleOrden | Si | Si | No | No | No |
| EstadoFactura | Si | Si | No | No | No |
| EstadoOrden | Si | Si | No | No | No |
| Factura | Si | Si | Si | Si | Si |
| Garantia | Si | Si | No | No | No |
| HistorialEstadoOrden | Si | Si | No | No | No |
| HistorialKilometraje | Si | Si | No | No | No |
| LogInventario | Si | Si | No | No | No |
| MarcaVehiculo | Si | Si | No | No | No |
| MetodoPago | Si | Si | No | No | No |
| ModeloVehiculo | Si | Si | No | No | No |
| NotaOrden | Si | Si | No | No | No |
| OrdenMecanico | Si | Si | No | No | No |
| OrdenServicio | Si | Si | Si | Si | Si |
| OrdenTipoServicio | Si | Si | No | No | No |
| Pago | Si | Si | No | No | No |
| Pais | Si | Si | No | No | No |
| Proveedor | Si | Si | No | No | No |
| Repuesto | Si | Si | Si | Si | Si |
| RepuestoProveedor | Si | Si | No | No | No |
| Rol | Si | Si | No | No | No |
| TareaMecanico | Si | Si | No | No | No |
| TipoServicio | Si | Si | No | No | No |
| UnidadMedida | Si | Si | No | No | No |
| Usuario | Si | Si | Si | Si | Si |
| Vehiculo | Si | Si | Si | Si | Si |

## Lectura rapida

Todas las entidades tienen `DbSet` en `AutoTallerDbContext`.

Los modulos con estructura completa basica son:

- `Cliente`
- `Vehiculo`
- `OrdenServicio`
- `Repuesto`
- `Factura`
- `Usuario`

Las entidades con configuracion EF pero sin modulo de aplicacion/API son:

- `Auditoria`
- `CategoriaRepuesto`
- `Cita`
- `Ciudad`
- `ClienteCorreo`
- `ClienteDireccion`
- `ClienteTelefono`
- `Compra`
- `Departamento`
- `DetalleCompra`
- `DetalleOrden`
- `EstadoFactura`
- `EstadoOrden`
- `Garantia`
- `HistorialEstadoOrden`
- `HistorialKilometraje`
- `LogInventario`
- `MarcaVehiculo`
- `MetodoPago`
- `ModeloVehiculo`
- `NotaOrden`
- `OrdenMecanico`
- `OrdenTipoServicio`
- `Pago`
- `Pais`
- `Proveedor`
- `RepuestoProveedor`
- `Rol`
- `TareaMecanico`
- `TipoServicio`
- `UnidadMedida`

Las entidades sin configuracion EF especifica todavia son:

- Ninguna.

## Prioridad sugerida

### 1. Completar configuraciones EF faltantes

Antes de crear mas controladores, conviene que todas las tablas tengan su `IEntityTypeConfiguration<T>`. Esto deja el modelo de base de datos controlado desde Fluent API y evita depender demasiado de convenciones implicitas de EF Core.

Orden recomendado:

1. Catalogos de ubicacion y vehiculos: `Pais`, `Departamento`, `Ciudad`, `MarcaVehiculo`.
2. Datos dependientes de cliente: `ClienteCorreo`, `ClienteDireccion`, `ClienteTelefono`.
3. Detalles de orden: `DetalleOrden`, `OrdenMecanico`, `OrdenTipoServicio`, `TareaMecanico`, `NotaOrden`, `HistorialEstadoOrden`.
4. Inventario y compras: `Proveedor`, `RepuestoProveedor`, `Compra`, `DetalleCompra`, `LogInventario`.
5. Facturacion y postventa: `Pago`, `Garantia`, `HistorialKilometraje`.

### 2. Crear modulos API para catalogos

Despues de las configuraciones, el siguiente bloque deberia ser CRUD simple para catalogos:

- `Roles`
- `EstadosOrden`
- `EstadosFactura`
- `TiposServicio`
- `CategoriasRepuesto`
- `UnidadesMedida`
- `MetodosPago`
- `MarcasVehiculo`
- `ModelosVehiculo`
- `Paises`
- `Departamentos`
- `Ciudades`

### 3. Crear modulos operativos

Luego avanzar con modulos que tienen reglas de negocio:

- `Citas`
- `OrdenesServicio` extendido con tipos de servicio, mecanicos, tareas, notas e historial.
- `Compras` y `DetalleCompra`.
- `Pagos`.
- `Auditorias`.

## Plantilla para llevar una entidad al mismo nivel

Para cada entidad nueva, replicar esta estructura:

```text
Domain/Entities/<Entidad>.cs
Infrastructure/Configuration/<Entidad>Configuration.cs
Application/Abstractions/I<Entidad>Repository.cs
Infrastructure/Repositories/<Entidad>Repository.cs
Application/<Modulo>/UseCase/Create<Entidad>.cs
Application/<Modulo>/UseCase/Update<Entidad>.cs
Api/<Modulo>/Dtos/<Entidad>Contracts.cs
Api/<Modulo>/Controllers/<Entidad>Controller.cs
```

Si la entidad es un detalle interno de otro agregado, no siempre necesita controlador propio. Por ejemplo, `DetalleOrden`, `OrdenMecanico` u `OrdenTipoServicio` pueden administrarse desde endpoints de `OrdenServicio`.
