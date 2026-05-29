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
| Auditoria | Si | Si | Si | Si | Si |
| CategoriaRepuesto | Si | Si | Si | Si | Si |
| Cita | Si | Si | Si | Si | Si |
| Ciudad | Si | Si | Si | Si | Si |
| Cliente | Si | Si | Si | Si | Si |
| ClienteCorreo | Si | Si | Si | Si | Si |
| ClienteDireccion | Si | Si | Si | Si | Si |
| ClienteTelefono | Si | Si | Si | Si | Si |
| Compra | Si | Si | Si | Si | Si |
| Departamento | Si | Si | Si | Si | Si |
| DetalleCompra | Si | Si | Si | Si | Si |
| DetalleOrden | Si | Si | Si | Si | Si |
| EstadoFactura | Si | Si | Si | Si | Si |
| EstadoOrden | Si | Si | Si | Si | Si |
| Factura | Si | Si | Si | Si | Si |
| Garantia | Si | Si | Si | Si | Si |
| HistorialEstadoOrden | Si | Si | Si | Si | Si |
| HistorialKilometraje | Si | Si | Si | Si | Si |
| LogInventario | Si | Si | Si | No | Si |
| MarcaVehiculo | Si | Si | Si | Si | Si |
| MetodoPago | Si | Si | Si | Si | Si |
| ModeloVehiculo | Si | Si | Si | Si | Si |
| NotaOrden | Si | Si | Si | Si | Si |
| OrdenMecanico | Si | Si | Si | Si | Si |
| OrdenServicio | Si | Si | Si | Si | Si |
| OrdenTipoServicio | Si | Si | Si | Si | Si |
| Pago | Si | Si | Si | Si | Si |
| Pais | Si | Si | Si | Si | Si |
| Proveedor | Si | Si | Si | Si | Si |
| Repuesto | Si | Si | Si | Si | Si |
| RepuestoProveedor | Si | Si | Si | Si | Si |
| Rol | Si | Si | Si | Si | Si |
| TareaMecanico | Si | Si | Si | Si | Si |
| TipoServicio | Si | Si | Si | Si | Si |
| UnidadMedida | Si | Si | Si | Si | Si |
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
- `Rol`
- `EstadoOrden`
- `EstadoFactura`
- `MetodoPago`
- `UnidadMedida`
- `CategoriaRepuesto`
- `TipoServicio`
- `Pais`
- `Departamento`
- `Ciudad`
- `MarcaVehiculo`
- `ModeloVehiculo`
- `ClienteCorreo`
- `ClienteDireccion`
- `ClienteTelefono`
- `Cita`
- `Proveedor`
- `RepuestoProveedor`
- `Compra`
- `DetalleCompra`
- `LogInventario`
- `Pago`
- `Garantia`
- `HistorialKilometraje`
- `OrdenMecanico`
- `OrdenTipoServicio`
- `DetalleOrden`
- `TareaMecanico`
- `NotaOrden`
- `HistorialEstadoOrden`
- `Auditoria`

Las entidades con configuracion EF pero sin modulo de aplicacion/API son:

- Ninguna.

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
