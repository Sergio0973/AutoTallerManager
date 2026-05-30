# Flujo de pruebas Swagger

Este flujo fue probado manualmente desde Swagger contra la API local.

URL local:

```text
http://localhost:5258/swagger
```

## 1. Base de usuarios

### Rol

```http
POST /api/Rol
```

```json
{
  "nombre": "Recepcionista",
  "descripcion": "Gestiona clientes, vehículos y órdenes de servicio"
}
```

Resultado probado: `201 Created`.

Validacion probada:

```json
{
  "nombre": "",
  "descripcion": ""
}
```

Resultado probado: `400 Bad Request`.

### Usuario

```http
POST /api/Usuario
```

```json
{
  "rolId": 4,
  "correo": "recepcionista@autotaller.com",
  "nombre": "Recepcionista Principal"
}
```

Resultado probado: `201 Created`.

### Usuario mecanico

Para validar reglas de rol, crear tambien el rol y usuario mecanico:

```http
POST /api/Rol
```

```json
{
  "nombre": "Mecanico",
  "descripcion": "Usuario encargado de ejecutar trabajos de taller"
}
```

Resultado probado: `201 Created`.

```http
POST /api/Usuario
```

```json
{
  "rolId": 5,
  "correo": "mecanico@autotaller.com",
  "nombre": "Mecanico Principal"
}
```

Resultado probado: `201 Created`.

Validacion probada con `rolId` inexistente:

```json
{
  "rolId": 9999,
  "correo": "prueba@correo.com",
  "nombre": "Usuario Prueba"
}
```

Resultado probado: `404 Not Found`.

## 2. Cliente y vehiculo

### Cliente

```http
POST /api/Cliente
```

```json
{
  "nombres": "Carlos",
  "apellidos": "Perez",
  "documento": "1002003004"
}
```

Resultado probado: `201 Created`.

### MarcaVehiculo

```http
POST /api/MarcaVehiculo
```

```json
{
  "nombre": "Toyota"
}
```

Resultado probado: `201 Created`.

### ModeloVehiculo

```http
POST /api/ModeloVehiculo
```

```json
{
  "marcaId": 1,
  "nombre": "Corolla",
  "anioDesde": 2020,
  "anioHasta": 2026
}
```

Resultado probado: `201 Created`.

### Vehiculo

```http
POST /api/Vehiculo
```

```json
{
  "clienteId": 2,
  "modeloId": 1,
  "vin": "1HGCM82633A004352",
  "anio": 2024,
  "placa": "ABC123",
  "color": "Rojo"
}
```

Resultado probado: `201 Created`.

## 3. Orden de servicio

### EstadoOrden

```http
POST /api/EstadoOrden
```

```json
{
  "nombre": "Pendiente",
  "descripcion": "Orden registrada y pendiente de iniciar"
}
```

Resultado probado: `201 Created`.

### TipoServicio

```http
POST /api/TipoServicio
```

```json
{
  "nombre": "Diagnostico general",
  "descripcion": "Revision inicial del vehiculo",
  "diasEstimados": 1
}
```

Resultado probado: `201 Created`.

### OrdenServicio

```http
POST /api/OrdenServicio
```

```json
{
  "vehiculoId": 2,
  "recepcionistaId": 3,
  "estadoId": 1,
  "citaId": null,
  "kilometrajeIngreso": 85000,
  "fechaIngreso": "2026-05-30",
  "fechaEstimada": "2026-05-31",
  "observaciones": "Cliente reporta ruido al encender"
}
```

Resultado probado: `201 Created`.

### OrdenTipoServicio

```http
POST /api/OrdenTipoServicio
```

```json
{
  "ordenId": 2,
  "tipoServicioId": 1
}
```

Resultado probado: `201 Created`.

### OrdenMecanico

```http
POST /api/OrdenMecanico
```

```json
{
  "ordenId": 2,
  "mecanicoId": 4,
  "fechaAsignacion": "2026-05-30"
}
```

Resultado probado: `201 Created`.

Validacion de rol probada:

```json
{
  "ordenId": 2,
  "mecanicoId": 3,
  "fechaAsignacion": "2026-05-30"
}
```

Resultado probado: `409 Conflict`, porque el usuario `3` tiene rol `Recepcionista` y no `Mecanico`.

### TareaMecanico

```http
POST /api/TareaMecanico
```

```json
{
  "ordenId": 2,
  "mecanicoId": 4,
  "tipoServicioId": 1,
  "descripcion": "Revision inicial del sistema de encendido",
  "horasTrabajadas": 1.5,
  "costoHora": 45000,
  "estado": "En proceso",
  "fechaInicio": "2026-05-30T17:19:25.669Z",
  "fechaFin": "2026-05-30T17:19:25.669Z"
}
```

Resultado probado: `201 Created`.

Validacion de rol probada:

```json
{
  "ordenId": 2,
  "mecanicoId": 3,
  "tipoServicioId": 1,
  "descripcion": "Prueba de rol incorrecto",
  "horasTrabajadas": 1,
  "costoHora": 45000,
  "estado": "En proceso",
  "fechaInicio": "2026-05-30T18:10:00Z",
  "fechaFin": "2026-05-30T19:10:00Z"
}
```

Resultado probado: `409 Conflict`, porque el usuario `3` tiene rol `Recepcionista` y no `Mecanico`.

### NotaOrden

```http
POST /api/NotaOrden
```

```json
{
  "ordenId": 2,
  "usuarioId": 3,
  "contenido": "Se realiza diagnóstico inicial y se detecta ruido en sistema de encendido."
}
```

Resultado probado: `201 Created`.

### HistorialEstadoOrden

```http
POST /api/HistorialEstadoOrden
```

```json
{
  "ordenId": 2,
  "estadoId": 1,
  "usuarioId": 3,
  "observacion": "Orden registrada y diagnóstico inicial documentado."
}
```

Resultado probado: `201 Created`.

## 4. Inventario y compras

### UnidadMedida

```http
POST /api/UnidadMedida
```

```json
{
  "nombre": "Unidad",
  "abreviatura": "und"
}
```

Resultado probado: `201 Created`.

### CategoriaRepuesto

```http
POST /api/CategoriaRepuesto
```

```json
{
  "nombre": "Encendido",
  "descripcion": "Componentes del sistema de encendido"
}
```

Resultado probado: `201 Created`.

### Ubicacion para proveedor

```http
POST /api/Pais
```

```json
{
  "nombre": "Colombia",
  "codigo": "CO"
}
```

```http
POST /api/Departamento
```

```json
{
  "paisId": 2,
  "nombre": "Antioquia"
}
```

```http
POST /api/Ciudad
```

```json
{
  "departamentoId": 2,
  "nombre": "Medellin"
}
```

Resultado probado: `201 Created` en los tres endpoints.

### Proveedor

```http
POST /api/Proveedor
```

```json
{
  "nombre": "Autopartes Central",
  "nit": "900123456",
  "telefono": "3009876543",
  "correo": "ventas@autopartescentral.com",
  "ciudadId": 2
}
```

Resultado probado: `201 Created`.

### Repuesto

```http
POST /api/Repuesto
```

```json
{
  "categoriaId": 5,
  "unidadId": 4,
  "codigo": "BUJ-001",
  "descripcion": "Bujia de encendido estándar",
  "stockActual": 0,
  "stockMinimo": 5,
  "precioUnitario": 18000
}
```

Resultado probado: `201 Created`.

### RepuestoProveedor

```http
POST /api/RepuestoProveedor
```

```json
{
  "repuestoId": 2,
  "proveedorId": 2,
  "precioCompra": 12000,
  "principal": true
}
```

Resultado probado: `201 Created`.

### Compra

```http
POST /api/Compra
```

```json
{
  "proveedorId": 2,
  "usuarioId": 3,
  "fechaCompra": "2026-05-30",
  "estado": "Recibida",
  "observaciones": "Compra inicial de repuestos de encendido"
}
```

Resultado probado: `201 Created`.

Nota: el campo correcto es `fechaCompra`, no `fecha`.

### DetalleCompra

```http
POST /api/DetalleCompra
```

```json
{
  "compraId": 2,
  "repuestoId": 2,
  "cantidad": 10,
  "precioUnitario": 12000
}
```

Resultado probado: `201 Created`.

Nota: el campo correcto es `precioUnitario`, no `costoUnitario`.

Validaciones posteriores probadas:

```http
GET /api/Repuesto/2
```

Resultado esperado:

```json
"stockActual": 10
```

```http
GET /api/Compra/2
```

Resultado esperado:

```json
"total": 120000
```

## 5. Consumo de inventario en orden

### DetalleOrden

```http
POST /api/DetalleOrden
```

```json
{
  "ordenId": 2,
  "repuestoId": 2,
  "usuarioId": 3,
  "cantidad": 2,
  "precioSnapshot": 18000
}
```

Resultado probado: `201 Created`.

Validacion posterior:

```http
GET /api/Repuesto/2
```

Resultado esperado:

```json
"stockActual": 8
```

## 6. Facturacion y pagos

### EstadoFactura

```http
POST /api/EstadoFactura
```

```json
{
  "nombre": "Emitida"
}
```

Resultado probado: `201 Created`.

### Factura

```http
POST /api/Factura
```

```json
{
  "ordenId": 2,
  "estadoFacturaId": 4,
  "usuarioId": 3,
  "descuento": 0,
  "impuestoPct": 19,
  "fechaEmision": "2026-05-30",
  "observaciones": "Factura generada por diagnóstico inicial y repuestos utilizados"
}
```

Resultado probado: `201 Created`.

Nota: en la creacion, el backend calcula automaticamente:

- `manoDeObra` desde `TareaMecanico`.
- `costoRepuestos` desde `DetalleOrden`.
- `subtotal = manoDeObra + costoRepuestos`.
- `total = (subtotal - descuento) + impuesto`.

Prueba concreta validada con una orden sin factura previa:

1. Crear `OrdenServicio` con `id = 3`.
2. Crear `TareaMecanico` para esa orden:

```json
{
  "ordenId": 3,
  "mecanicoId": 4,
  "tipoServicioId": 1,
  "descripcion": "Trabajo para prueba de factura automática",
  "horasTrabajadas": 2,
  "costoHora": 50000,
  "estado": "Completada",
  "fechaInicio": "2026-05-30T18:30:00Z",
  "fechaFin": "2026-05-30T20:30:00Z"
}
```

3. Crear `DetalleOrden` para esa orden:

```json
{
  "ordenId": 3,
  "repuestoId": 2,
  "usuarioId": 3,
  "cantidad": 1,
  "precioSnapshot": 18000
}
```

4. Crear factura:

```json
{
  "ordenId": 3,
  "estadoFacturaId": 4,
  "usuarioId": 3,
  "descuento": 0,
  "impuestoPct": 19,
  "fechaEmision": "2026-05-30",
  "observaciones": "Factura automática de prueba"
}
```

Resultado validado:

```json
{
  "manoDeObra": 100000,
  "costoRepuestos": 18000,
  "descuento": 0,
  "impuestoPct": 19,
  "subtotal": 118000,
  "total": 140420
}
```

### MetodoPago

```http
POST /api/MetodoPago
```

```json
{
  "nombre": "Efectivo",
  "descripcion": "Pago realizado en efectivo"
}
```

Resultado probado: `201 Created`.

### Pago

```http
POST /api/Pago
```

```json
{
  "facturaId": 2,
  "metodoPagoId": 4,
  "monto": 123165,
  "referencia": "PAGO-FACT-002",
  "estado": "Confirmado"
}
```

Resultado probado: `201 Created`.

## 7. Trazabilidad

### LogInventario

```http
GET /api/LogInventario?repuestoId=2
```

Resultado probado:

- `ENTRADA_COMPRA` con `cantidad: 10` y `stockResultante: 10`.
- `SALIDA_ORDEN` con `cantidad: -2` y `stockResultante: 8`.

### Auditoria

La auditoria se registra automaticamente al crear:

- `OrdenServicio`
- `Factura`
- `Pago`

Para verificarlo, despues de crear una orden, factura o pago, consultar:

```http
GET /api/Auditoria
```

Debe aparecer un registro con:

- `entidad`: `OrdenServicio`, `Factura` o `Pago`.
- `tipoAccion`: `CREAR`.
- `datosNuevos`: JSON con los datos principales del registro creado.
- `ipOrigen`: `SYSTEM`.

Tambien se puede crear una auditoria manual para pruebas:

```http
POST /api/Auditoria
```

```json
{
  "usuarioId": 3,
  "entidad": "OrdenServicio",
  "entidadId": 2,
  "tipoAccion": "PRUEBA_FLUJO",
  "datosAnteriores": null,
  "datosNuevos": "{\"estado\":\"flujo probado desde Swagger\"}",
  "ipOrigen": "127.0.0.1"
}
```

Resultado probado: `201 Created`.

## Observaciones detectadas

- Ejecutar `docs/sql/sincronizar-secuencias-postgres.sql` si PostgreSQL intenta reutilizar IDs existentes.
- El backend ya valida relaciones principales antes de guardar para evitar errores `500` por llaves foraneas.
- Quedan mejoras recomendadas:
  - Validacion de roles reales para recepcionista, mecanico y admin. Parcialmente implementada y probada para `Recepcionista`/`Mecanico` en ordenes, citas, tareas y garantias.
  - Calculo automatico de factura desde tareas y repuestos. Implementado para creacion de facturas.
  - Auditoria automatica para operaciones importantes. Implementada para creacion de ordenes, facturas y pagos.
  - Rutas REST en plural si se quiere normalizar la API publica.
