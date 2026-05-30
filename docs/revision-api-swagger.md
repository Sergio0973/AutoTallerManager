# Revision API y Swagger

## Resultado

La API compila correctamente y Swagger queda disponible en desarrollo en:

```text
http://localhost:5258/swagger
```

El perfil local usado es `http`, definido en `Api/Properties/launchSettings.json`.

## Rutas actuales

Las rutas se generan desde `BaseApiController` con:

```csharp
[Route("api/[controller]")]
```

Por eso las rutas quedan en singular, tomando el nombre del controlador:

```text
api/Auditoria
api/CategoriaRepuesto
api/Cita
api/Ciudad
api/Cliente
api/ClienteCorreo
api/ClienteDireccion
api/ClienteTelefono
api/Compra
api/Departamento
api/DetalleCompra
api/DetalleOrden
api/EstadoFactura
api/EstadoOrden
api/Factura
api/Garantia
api/HistorialEstadoOrden
api/HistorialKilometraje
api/LogInventario
api/MarcaVehiculo
api/MetodoPago
api/ModeloVehiculo
api/NotaOrden
api/OrdenMecanico
api/OrdenServicio
api/OrdenTipoServicio
api/Pago
api/Pais
api/Proveedor
api/Repuesto
api/RepuestoProveedor
api/Rol
api/TareaMecanico
api/TipoServicio
api/UnidadMedida
api/Usuario
api/Vehiculo
```

## Limpieza realizada

Se eliminaron los archivos de plantilla:

```text
Api/Controllers/WeatherForecastController.cs
Api/WeatherForecast.cs
```

Ese endpoint no pertenece al dominio de AutoTallerManager y no debe aparecer en Swagger.

## Flujo recomendado para probar

1. Crear catalogos base:
   - `Rol`
   - `EstadoOrden`
   - `EstadoFactura`
   - `MetodoPago`
   - `UnidadMedida`
   - `CategoriaRepuesto`
   - `TipoServicio`

2. Crear ubicacion y vehiculo:
   - `Pais`
   - `Departamento`
   - `Ciudad`
   - `MarcaVehiculo`
   - `ModeloVehiculo`
   - `Cliente`
   - `Vehiculo`

3. Crear operacion de taller:
   - `Usuario`
   - `OrdenServicio`
   - `OrdenMecanico`
   - `OrdenTipoServicio`
   - `TareaMecanico`
   - `NotaOrden`
   - `HistorialEstadoOrden`

4. Probar inventario y facturacion:
   - `Proveedor`
   - `Repuesto`
   - `RepuestoProveedor`
   - `Compra`
   - `DetalleCompra`
   - `DetalleOrden`
   - `Factura`
   - `Pago`

5. Revisar trazabilidad:
   - `LogInventario`
   - `Auditoria`

## Observaciones

- Si se quiere una API mas REST convencional, el siguiente ajuste seria pasar las rutas a plural, por ejemplo `/api/clientes` en vez de `/api/Cliente`.
- Ese cambio conviene hacerlo de forma controlada porque afectaria todos los endpoints publicados.
- Si PostgreSQL devuelve una llave primaria duplicada al crear un registro nuevo, normalmente la secuencia de IDs quedo atrasada frente a los datos existentes. Ejecutar `docs/sql/sincronizar-secuencias-postgres.sql` en la base de datos para alinear las secuencias con el `MAX(Id)` de cada tabla.
