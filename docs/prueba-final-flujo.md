# Prueba final de flujo completo

Fecha de prueba: 2026-06-01

Ambiente: API local con Swagger.

URL:

```text
http://localhost:5258/swagger
```

## Objetivo

Validar el flujo principal de AutoTallerManager desde la creacion de cliente hasta la trazabilidad de inventario y auditoria.

## Autenticacion

Usuario usado:

```json
{
  "correo": "admin.seed@autotaller.com",
  "contrasena": "Admin123!"
}
```

Resultado esperado y obtenido:

```text
200 OK
Token JWT generado correctamente.
```

## Flujo probado

### 1. Cliente

Endpoint:

```http
POST /api/Cliente
```

Resultado obtenido:

```json
{
  "id": 3,
  "nombres": "Laura",
  "apellidos": "Gomez",
  "documento": "5566778899",
  "activo": true
}
```

Estado: correcto.

### 2. Vehiculo

Endpoint:

```http
POST /api/Vehiculo
```

Datos clave:

```text
clienteId = 3
modeloId = 2
vin = 1HGCM82633A009991
placa = TST991
```

Resultado obtenido:

```json
{
  "id": 3,
  "clienteId": 3,
  "modeloId": 2,
  "vin": "1HGCM82633A009991",
  "anio": 2023,
  "placa": "TST991",
  "color": "Azul",
  "activo": true
}
```

Estado: correcto.

### 3. Orden de servicio

Endpoint:

```http
POST /api/OrdenServicio
```

Datos clave:

```text
vehiculoId = 3
recepcionistaId = 8
estadoId = 1
```

Resultado obtenido:

```json
{
  "id": 7,
  "vehiculoId": 3,
  "recepcionistaId": 8,
  "estadoId": 1,
  "kilometrajeIngreso": 45000,
  "fechaIngreso": "2026-06-01",
  "fechaEstimada": "2026-06-02",
  "observaciones": "Prueba final de flujo completo"
}
```

Estado: correcto.

### 4. Asignacion de mecanico

Endpoint:

```http
POST /api/OrdenMecanico
```

Datos clave:

```text
ordenId = 7
mecanicoId = 9
```

Resultado obtenido:

```json
{
  "id": 4,
  "ordenId": 7,
  "mecanicoId": 9,
  "fechaAsignacion": "2026-06-01"
}
```

Estado: correcto.

### 5. Tarea mecanica

Endpoint:

```http
POST /api/TareaMecanico
```

Datos clave:

```text
ordenId = 7
mecanicoId = 9
tipoServicioId = 1
horasTrabajadas = 2
costoHora = 50000
```

Resultado obtenido:

```json
{
  "id": 6,
  "ordenId": 7,
  "mecanicoId": 9,
  "tipoServicioId": 1,
  "descripcion": "Revision general y prueba final del flujo",
  "horasTrabajadas": 2,
  "costoHora": 50000,
  "costoTotal": 100000,
  "estado": "Completada"
}
```

Estado: correcto.

### 6. Detalle de orden e inventario

Repuesto usado:

```text
repuestoId = 2
descripcion = Bujia de encendido estandar
stock antes = 6
precioUnitario = 18000
```

Endpoint:

```http
POST /api/DetalleOrden
```

Resultado obtenido:

```json
{
  "id": 5,
  "ordenId": 7,
  "repuestoId": 2,
  "cantidad": 1,
  "precioSnapshot": 18000,
  "subtotal": 18000
}
```

Verificacion posterior:

```http
GET /api/Repuesto/2
```

Resultado clave:

```json
{
  "id": 2,
  "stockActual": 5,
  "stockMinimo": 5
}
```

Estado: correcto. El stock bajo de `6` a `5`.

### 7. Factura

Endpoint:

```http
POST /api/Factura
```

Datos clave:

```text
ordenId = 7
estadoFacturaId = 4
usuarioId = 8
manoDeObra esperada = 100000
costoRepuestos esperado = 18000
```

Resultado obtenido:

```json
{
  "id": 5,
  "ordenId": 7,
  "estadoFacturaId": 4,
  "usuarioId": 8,
  "manoDeObra": 100000,
  "costoRepuestos": 18000,
  "descuento": 0,
  "impuestoPct": 19,
  "subtotal": 118000,
  "total": 140420,
  "fechaEmision": "2026-06-01",
  "observaciones": "Factura prueba final de flujo completo"
}
```

Estado: correcto. Totales calculados correctamente.

### 8. Pago

Endpoint:

```http
POST /api/Pago
```

Datos clave:

```text
facturaId = 5
metodoPagoId = 4
monto = 140420
referencia = PAGO-FLUJO-FINAL-005
estado = Confirmado
```

Resultado obtenido:

```json
{
  "id": 4,
  "facturaId": 5,
  "metodoPagoId": 4,
  "monto": 140420,
  "referencia": "PAGO-FLUJO-FINAL-005",
  "estado": "Confirmado"
}
```

Estado: correcto.

## Trazabilidad

### Auditoria de factura

Endpoint:

```http
GET /api/Auditoria?usuarioId=8&entidad=Factura&tipoAccion=CREAR
```

Resultado clave:

```text
entidad = Factura
entidadId = 5
tipoAccion = CREAR
usuarioId = 8
```

Estado: correcto.

### Auditoria de pago

Endpoint:

```http
GET /api/Auditoria?usuarioId=8&entidad=Pago&tipoAccion=CREAR
```

Resultado clave:

```text
entidad = Pago
entidadId = 4
tipoAccion = CREAR
usuarioId = 8
```

Estado: correcto.

### Log de inventario

Endpoint:

```http
GET /api/LogInventario?repuestoId=2&usuarioId=9
```

Resultado clave:

```json
{
  "id": 6,
  "repuestoId": 2,
  "usuarioId": 9,
  "ordenId": 7,
  "tipoMovimiento": "SALIDA_ORDEN",
  "cantidad": -1,
  "stockResultante": 5,
  "motivo": "Salida de inventario por orden de servicio."
}
```

Estado: correcto.

## Resultado final

Flujo completo validado correctamente:

- Cliente creado.
- Vehiculo creado.
- Orden de servicio creada.
- Mecanico asignado.
- Tarea mecanica registrada.
- Repuesto usado y stock descontado.
- Factura generada con calculos correctos.
- Pago confirmado.
- Auditoria registrada para orden, factura y pago.
- Log de inventario registrado.

Estado general de la prueba: exitoso.
