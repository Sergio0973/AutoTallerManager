# Flujo de pruebas Swagger

Este flujo fue probado manualmente desde Swagger contra la API local.

URL local:

```text
http://localhost:5258/swagger
```

## Estado de migraciones

Verificacion ejecutada:

```bash
dotnet build --no-restore
dotnet ef migrations has-pending-model-changes -p .\Infrastructure -s .\Api
```

Resultado:

```text
Compilacion correcta.
No changes have been made to the model since the last migration.
```

No se requiere crear una nueva migracion por los ultimos cambios, porque fueron reglas de aplicacion, documentacion Swagger y datos semilla.

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
  "nombre": "Recepcionista Principal",
  "contrasena": "Recepcionista123!"
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
  "nombre": "Mecanico Principal",
  "contrasena": "Mecanico123!"
}
```

Resultado probado: `201 Created`.

Validacion probada con `rolId` inexistente:

```json
{
  "rolId": 9999,
  "correo": "prueba@correo.com",
  "nombre": "Usuario Prueba",
  "contrasena": "Usuario123!"
}
```

Resultado probado: `404 Not Found`.

### Login JWT

```http
POST /api/Auth/login
```

```json
{
  "correo": "recepcionista@autotaller.com",
  "contrasena": "Recepcionista123!"
}
```

Resultado esperado: `200 OK` con:

- `token`
- `expiraEn`
- datos del usuario autenticado

En Swagger, usar el boton `Authorize` y pegar:

```text
Bearer {token}
```

Notas:

- Los usuarios creados antes de agregar `PasswordHash` quedan sin contrasena y no pueden iniciar sesion.
- Para probar login, crear un usuario nuevo enviando `contrasena`.

### Datos semilla

Al iniciar la API, el sistema crea datos base si no existen:

- Roles: `Admin`, `Mecanico`, `Recepcionista`.
- Estados de orden: `Pendiente`, `En proceso`, `Completada`, `Cancelada`.
- Estados de factura: `Emitida`, `Pagada`, `Anulada`.
- Metodos de pago: `Efectivo`, `Tarjeta`, `Transferencia`.
- Tipos de servicio: `Diagnostico`, `Mantenimiento preventivo`, `Reparacion`.
- Inventario base: categorias `Encendido`, `Frenos`, `Filtros` y unidades `Unidad`, `Litro`.

En ambiente `Development` tambien crea usuarios de prueba si no existen:

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

### Reset de contrasena por Admin

```http
PUT /api/Usuario/{id}/password
```

Requiere token `Admin`.

```json
{
  "nuevaContrasena": "Nueva123!"
}
```

Resultado esperado: `204 No Content`.

Prueba recomendada:

1. Login como `Admin`.
2. Ejecutar `PUT /api/Usuario/7/password`.
3. Hacer login con el usuario `7` usando la nueva contrasena.

### Autorizacion por roles

Primera tanda protegida con politica `Admin`:

- `Usuario`
- `Rol`
- `Auditoria`
- `Repuesto`
- `CategoriaRepuesto`
- `UnidadMedida`
- `Compra`
- `DetalleCompra`
- `Proveedor`
- `RepuestoProveedor`
- `LogInventario`
- `EstadoFactura`
- `MetodoPago`

Prueba recomendada:

1. Sin token, ejecutar `GET /api/Usuario`: debe responder `401 Unauthorized`.
2. Con token de `Recepcionista`, ejecutar `GET /api/Usuario`: debe responder `403 Forbidden`.
3. Crear rol `Admin` si no existe. Este paso solo funciona con token `Admin`; durante el bootstrap inicial se dejo temporalmente publico y luego se cerro:

```http
POST /api/Rol
```

```json
{
  "nombre": "Admin",
  "descripcion": "Acceso total al sistema"
}
```

4. Crear usuario admin con el `id` real del rol `Admin`. Este paso solo funciona con token `Admin`; durante el bootstrap inicial se dejo temporalmente publico y luego se cerro:

```http
POST /api/Usuario
```

```json
{
  "rolId": 6,
  "correo": "admin@autotaller.com",
  "nombre": "Administrador Principal",
  "contrasena": "Admin123!"
}
```

5. Iniciar sesion con `POST /api/Auth/login`, autorizar Swagger con `Bearer {token}` y repetir `GET /api/Usuario`: debe responder `200 OK`.

Segunda tanda protegida con politica `Recepcionista` (`Recepcionista` o `Admin`):

- `Cliente`
- `ClienteCorreo`
- `ClienteTelefono`
- `ClienteDireccion`
- `Vehiculo`
- `HistorialKilometraje`
- `Cita`
- `OrdenServicio`

Prueba recomendada:

1. Sin token, ejecutar `GET /api/Cliente`: debe responder `401 Unauthorized`.
2. Con token `Mecanico`, ejecutar `GET /api/Cliente`: debe responder `403 Forbidden`.
3. Con token `Recepcionista`, ejecutar `GET /api/Cliente`: debe responder `200 OK`.
4. Con token `Admin`, ejecutar `GET /api/Cliente`: debe responder `200 OK`.

Tercera tanda protegida con politica `Mecanico` (`Mecanico` o `Admin`):

- `OrdenMecanico`
- `TareaMecanico`
- `NotaOrden`
- `HistorialEstadoOrden`
- `DetalleOrden`
- `Garantia`
- `Factura`
- `Pago`

Prueba recomendada:

1. Sin token, ejecutar `GET /api/TareaMecanico`: debe responder `401 Unauthorized`.
2. Con token `Recepcionista`, ejecutar `GET /api/TareaMecanico`: debe responder `403 Forbidden`.
3. Con token `Mecanico`, ejecutar `GET /api/TareaMecanico`: debe responder `200 OK`.
4. Con token `Admin`, ejecutar `GET /api/TareaMecanico`: debe responder `200 OK`.

Cuarta tanda de cierre:

Catalogos/configuracion protegidos con politica `Admin`:

- `Pais`
- `Departamento`
- `Ciudad`
- `MarcaVehiculo`
- `ModeloVehiculo`
- `EstadoOrden`
- `TipoServicio`

Relacion de servicios de una orden protegida con politica `Recepcionista`:

- `OrdenTipoServicio`

Verificacion tecnica: no quedan controladores sin `[Authorize]` o `[AllowAnonymous]`, excepto controladores base comunes.

Pruebas realizadas:

- `GET /api/Pais` sin token: `401 Unauthorized`.
- `GET /api/Pais` con token `Recepcionista`: `403 Forbidden`.
- `GET /api/Pais` con token `Admin`: `200 OK`.
- `GET /api/OrdenTipoServicio` con token `Recepcionista`: `200 OK`.

Estado final del bloque JWT/autorizacion:

- `POST /api/Auth/login`: publico.
- `Admin`: usuarios, roles, auditoria, inventario y catalogos de configuracion.
- `Recepcionista` o `Admin`: clientes, vehiculos, citas, ordenes de servicio y tipos de servicio asignados a orden.
- `Mecanico` o `Admin`: asignaciones, tareas, notas, historial de estado, detalle de repuestos usados, garantias, facturas y pagos.

## 8. Borrado seguro

Primera tanda implementada:

- `DELETE /api/Cliente/{id}` devuelve `409 Conflict` si el cliente tiene vehiculos, telefonos, correos o direcciones asociadas.
- `DELETE /api/Vehiculo/{id}` devuelve `409 Conflict` si el vehiculo tiene citas, ordenes de servicio o historial de kilometraje asociado.
- `DELETE /api/Rol/{id}` devuelve `409 Conflict` si el rol tiene usuarios asociados.
- `DELETE /api/Repuesto/{id}` devuelve `409 Conflict` si el repuesto tiene compras, ordenes, proveedores o logs de inventario asociados.

Prueba recomendada:

- Con token `Admin`, intentar eliminar `Rol` con `id = 6`: debe responder `409 Conflict`.
- Con token `Recepcionista`, intentar eliminar `Cliente` con `id = 2`: debe responder `409 Conflict`.
- Con token `Recepcionista`, intentar eliminar `Vehiculo` con `id = 2`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `Repuesto` con `id = 2`: debe responder `409 Conflict`.

Segunda tanda implementada:

- `DELETE /api/Pais/{id}` devuelve `409 Conflict` si el pais tiene departamentos asociados.
- `DELETE /api/Departamento/{id}` devuelve `409 Conflict` si el departamento tiene ciudades asociadas.
- `DELETE /api/Ciudad/{id}` devuelve `409 Conflict` si la ciudad tiene proveedores o direcciones de cliente asociadas.
- `DELETE /api/MarcaVehiculo/{id}` devuelve `409 Conflict` si la marca tiene modelos asociados.
- `DELETE /api/ModeloVehiculo/{id}` devuelve `409 Conflict` si el modelo tiene vehiculos asociados.
- `DELETE /api/EstadoOrden/{id}` devuelve `409 Conflict` si el estado tiene ordenes o historial asociado.
- `DELETE /api/TipoServicio/{id}` devuelve `409 Conflict` si el tipo de servicio tiene citas, ordenes, tareas o garantias asociadas.
- `DELETE /api/Proveedor/{id}` devuelve `409 Conflict` si el proveedor tiene compras o repuestos asociados.

Prueba recomendada:

- Con token `Admin`, intentar eliminar `Pais` con `id = 2`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `Ciudad` con `id = 2`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `MarcaVehiculo` con `id = 1`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `TipoServicio` con `id = 1`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `Proveedor` con `id = 2`: debe responder `409 Conflict`.

Tercera tanda implementada:

- `DELETE /api/OrdenServicio/{id}` devuelve `409 Conflict` si la orden tiene servicios, mecanicos, tareas, detalles, notas, historial, factura, garantia o logs asociados.
- `DELETE /api/Compra/{id}` devuelve `409 Conflict` si la compra tiene detalles o logs de inventario asociados.
- `DELETE /api/Factura/{id}` devuelve `409 Conflict` si la factura tiene pagos asociados.
- `DELETE /api/Usuario/{id}` devuelve `409 Conflict` si el usuario tiene registros operativos o auditorias asociadas.
- `DELETE /api/CategoriaRepuesto/{id}` devuelve `409 Conflict` si la categoria tiene repuestos asociados.
- `DELETE /api/UnidadMedida/{id}` devuelve `409 Conflict` si la unidad tiene repuestos asociados.
- `DELETE /api/EstadoFactura/{id}` devuelve `409 Conflict` si el estado tiene facturas asociadas.
- `DELETE /api/MetodoPago/{id}` devuelve `409 Conflict` si el metodo tiene pagos asociados.

Prueba recomendada:

- Con token `Recepcionista`, intentar eliminar `OrdenServicio` con `id = 5`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `Compra` con `id = 2`: debe responder `409 Conflict`.
- Con token `Mecanico`, intentar eliminar `Factura` con `id = 4`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `Usuario` con `id = 6`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `CategoriaRepuesto` con `id = 5`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `UnidadMedida` con `id = 4`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `EstadoFactura` con `id = 4`: debe responder `409 Conflict`.
- Con token `Admin`, intentar eliminar `MetodoPago` con `id = 4`: debe responder `409 Conflict`.

## 9. Reglas de negocio de pagos y facturas

Reglas implementadas:

- `PUT /api/Pago/{id}` devuelve `409 Conflict` si el pago actual esta en estado `Confirmado`.
- `DELETE /api/Pago/{id}` devuelve `409 Conflict` si el pago esta en estado `Confirmado`.
- `PUT /api/Factura/{id}` devuelve `409 Conflict` si la factura tiene pagos confirmados.

Prueba recomendada:

- Con token `Mecanico`, intentar modificar `Pago` con `id = 3`: debe responder `409 Conflict`.
- Con token `Mecanico`, intentar eliminar `Pago` con `id = 3`: debe responder `409 Conflict`.
- Con token `Mecanico`, intentar modificar `Factura` con `id = 4`: debe responder `409 Conflict`.

Nota: los detalles de compra y de orden ya ajustan/revierten inventario dentro de transacciones y registran `LogInventario`. Si el reverso deja stock insuficiente, el dominio bloquea el movimiento.

## 10. Auditoria automatica de actualizaciones

Se registra `ACTUALIZAR` automaticamente en:

- `PUT /api/OrdenServicio/{id}`
- `PUT /api/Factura/{id}`
- `PUT /api/Pago/{id}`

Cada registro guarda:

- `datosAnteriores`: snapshot antes del cambio.
- `datosNuevos`: snapshot despues del cambio.
- `ipOrigen`: `SYSTEM`.

Prueba recomendada:

- Actualizar una orden de servicio sin romper sus relaciones y consultar `GET /api/Auditoria?entidad=OrdenServicio`.
- Actualizar una factura sin pagos confirmados y consultar `GET /api/Auditoria?entidad=Factura`.
- Actualizar un pago no confirmado y consultar `GET /api/Auditoria?entidad=Pago`.

## 11. Paginacion

Implementado en listados principales:

```http
GET /api/Cliente?pageNumber=1&pageSize=20&search=Carlos
GET /api/Vehiculo?pageNumber=1&pageSize=20&clienteId=2
GET /api/Vehiculo?pageNumber=1&pageSize=20&vin=1HGCM82633A004352
GET /api/Vehiculo?pageNumber=1&pageSize=20&placa=ABC123
GET /api/OrdenServicio?pageNumber=1&pageSize=20&estadoId=1
GET /api/Repuesto?pageNumber=1&pageSize=20&categoriaId=5
GET /api/Repuesto?pageNumber=1&pageSize=20&search=bujia
GET /api/Repuesto?pageNumber=1&pageSize=20&stockMinimo=5
GET /api/Repuesto?pageNumber=1&pageSize=20&soloBajoStock=true
```

Comportamiento:

- `pageNumber` por defecto: `1`.
- `pageSize` por defecto: `20`.
- `search` filtra donde el repositorio tenga soporte de busqueda.
- `estadoId` filtra ordenes de servicio por estado.
- `vehiculoId` filtra ordenes por vehiculo.
- `recepcionistaId` filtra ordenes por recepcionista.
- `fechaIngresoDesde` y `fechaIngresoHasta` filtran ordenes por rango de fecha de ingreso.
- `clienteId` filtra vehiculos por cliente.
- `vin` filtra vehiculos por numero de serie.
- `placa` filtra vehiculos por placa.
- `categoriaId` filtra repuestos por categoria.
- `stockMinimo` filtra repuestos cuyo stock actual sea menor o igual al valor indicado.
- `soloBajoStock=true` filtra repuestos cuyo stock actual sea menor o igual al stock minimo configurado.
- Responde header `X-Total-Count` con el total de registros que cumplen el filtro.
- Si `pageNumber` o `pageSize` son menores o iguales a cero, responde `400 Bad Request`.

Prueba recomendada:

- Con token `Recepcionista`, ejecutar `GET /api/Cliente?pageNumber=1&pageSize=10`.
- Verificar respuesta `200 OK`.
- Revisar en response headers el valor `X-Total-Count`.
- Repetir con `GET /api/Vehiculo?pageNumber=1&pageSize=10&clienteId=2`.
- Repetir con `GET /api/Vehiculo?pageNumber=1&pageSize=10&vin=1HGCM82633A004352`.
- Repetir con `GET /api/OrdenServicio?pageNumber=1&pageSize=10`.
- Repetir con `GET /api/OrdenServicio?pageNumber=1&pageSize=10&vehiculoId=2&fechaIngresoDesde=2026-05-30&fechaIngresoHasta=2026-05-31`.
- Con token `Admin`, repetir con `GET /api/Repuesto?pageNumber=1&pageSize=10`.
- Con token `Admin`, repetir con `GET /api/Repuesto?pageNumber=1&pageSize=10&soloBajoStock=true`.

## 12. Rate limiting

Implementado con el middleware nativo de ASP.NET Core:

- `OrdenServicio`: maximo `60` solicitudes por minuto.
- `Repuesto`: maximo `30` solicitudes por minuto.

Endpoints afectados:

- `/api/OrdenServicio`
- `/api/Repuesto`

Comportamiento esperado al exceder el limite:

```text
429 Too Many Requests
```

Nota: para probarlo manualmente desde Swagger hay que repetir muchas solicitudes en menos de un minuto. Es mas practico validarlo con una prueba automatizada o un script.

## 13. Reglas de estado de orden

Estados terminales:

- `Completada`
- `Cancelada`

Reglas implementadas:

- No se puede crear una orden nueva para un vehiculo que ya tenga una orden activa.
- No se puede crear o actualizar una cita para un vehiculo que ya tenga una orden activa.
- No se puede modificar una orden en estado `Completada` o `Cancelada`.
- No se puede asignar un mecanico a una orden si ya tiene otra orden activa.
- No se puede asignar mecanico a una orden `Completada` o `Cancelada`.
- No se puede crear tarea mecanica en una orden `Completada` o `Cancelada`.
- No se puede agregar detalle de repuesto a una orden `Completada` o `Cancelada`.
- No se puede crear nota en una orden `Completada` o `Cancelada`.
- No se puede crear historial de estado en una orden `Completada` o `Cancelada`.
- No se puede crear garantia en una orden `Completada` o `Cancelada`.
- No se puede facturar una orden `Cancelada`.

Comportamiento esperado:

```text
409 Conflict
```

Nota: las comparaciones se hacen por nombre de estado, no por `Id`, para no depender de IDs fijos en la base de datos.

### Prueba de orden activa por vehiculo

Requiere token `Recepcionista` o `Admin`.

1. Crear una orden para un vehiculo existente usando un estado no terminal, por ejemplo `Pendiente` o `En proceso`.
2. Intentar crear otra orden para el mismo `vehiculoId` sin completar o cancelar la anterior.

Body de ejemplo:

```json
{
  "vehiculoId": 2,
  "recepcionistaId": 3,
  "estadoId": 1,
  "citaId": null,
  "kilometrajeIngreso": 90000,
  "fechaIngreso": "2026-05-31",
  "fechaEstimada": "2026-06-01",
  "observaciones": "Prueba de bloqueo por orden activa"
}
```

Resultado esperado:

```json
{
  "message": "El vehiculo ya tiene una orden de servicio activa."
}
```

### Prueba de disponibilidad de mecanico

Requiere token `Mecanico` o `Admin`.

1. Identificar un mecanico que ya este asignado a una orden activa.
2. Intentar asignar ese mismo mecanico a otra orden activa.

Body de ejemplo:

```json
{
  "ordenId": 5,
  "mecanicoId": 4,
  "fechaAsignacion": "2026-05-31"
}
```

Resultado esperado:

```json
{
  "message": "El mecanico ya tiene una orden de servicio activa."
}
```

### Prueba de cita bloqueada por orden activa

Requiere token `Recepcionista` o `Admin`.

Endpoint:

```http
POST /api/Cita
```

Body de ejemplo:

```json
{
  "vehiculoId": 2,
  "recepcionistaId": 3,
  "tipoServicioId": 1,
  "fechaCita": "2026-06-02",
  "horaInicio": "09:00:00",
  "horaFin": "10:00:00",
  "estado": "Programada",
  "observaciones": "Prueba de bloqueo de cita por orden activa"
}
```

Si el vehiculo ya tiene una orden activa, resultado esperado:

```json
{
  "message": "El vehiculo ya tiene una orden de servicio activa."
}
```

### Prueba de cita duplicada por horario

Requiere un vehiculo sin orden activa.

1. Crear una cita para un vehiculo disponible.
2. Crear otra cita para el mismo `vehiculoId`, la misma `fechaCita` y un horario cruzado.

Resultado esperado:

```json
{
  "message": "El vehiculo ya tiene una cita en ese horario."
}
```

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

Tambien se registra automaticamente al actualizar:

- `OrdenServicio`
- `Factura`
- `Pago`

Para verificarlo, despues de crear una orden, factura o pago, consultar:

```http
GET /api/Auditoria
```

Debe aparecer un registro con:

- `entidad`: `OrdenServicio`, `Factura` o `Pago`.
- `tipoAccion`: `CREAR` o `ACTUALIZAR`.
- `datosNuevos`: JSON con los datos principales del registro creado.
- `datosAnteriores`: JSON con los datos anteriores en actualizaciones.
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
