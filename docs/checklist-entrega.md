# Checklist de entrega - AutoTallerManager

Este checklist resume el estado del proyecto frente a los requisitos funcionales y tecnicos definidos para AutoTallerManager.

## Arquitectura

- [x] Proyecto organizado por capas: `Api`, `Application`, `Domain`, `Infrastructure`.
- [x] Entidades de dominio centralizadas en `Domain/Entities`.
- [x] Casos de uso en `Application/<Modulo>/UseCase`.
- [x] Abstracciones de repositorios en `Application/Abstractions`.
- [x] Persistencia con EF Core en `Infrastructure`.
- [x] Controladores REST en `Api`.
- [x] Patron Repository y Unit of Work implementado.
- [x] MediatR usado para orquestar casos de uso.
- [x] FluentValidation usado para validaciones de requests/casos de uso.

## Modelo y persistencia

- [x] Todas las entidades tienen `DbSet` en `AutoTallerDbContext`.
- [x] Todas las entidades tienen configuracion EF Core con Fluent API.
- [x] Todas las entidades tienen repositorio o acceso definido.
- [x] Todas las entidades tienen modulo API o endpoint de consulta donde aplica.
- [x] Migraciones sincronizadas con el modelo actual.
- [x] Verificado con `dotnet ef migrations has-pending-model-changes -p .\Infrastructure -s .\Api`.
- [x] Script de sincronizacion de secuencias PostgreSQL disponible en `docs/sql/sincronizar-secuencias-postgres.sql`.

## Modulos funcionales

- [x] Clientes.
- [x] Correos de cliente.
- [x] Telefonos de cliente.
- [x] Direcciones de cliente.
- [x] Vehiculos.
- [x] Marcas de vehiculo.
- [x] Modelos de vehiculo.
- [x] Paises.
- [x] Departamentos.
- [x] Ciudades.
- [x] Citas.
- [x] Ordenes de servicio.
- [x] Tipos de servicio por orden.
- [x] Asignacion de mecanicos.
- [x] Tareas de mecanico.
- [x] Notas de orden.
- [x] Historial de estados de orden.
- [x] Repuestos.
- [x] Categorias de repuesto.
- [x] Unidades de medida.
- [x] Proveedores.
- [x] Proveedores por repuesto.
- [x] Compras.
- [x] Detalles de compra.
- [x] Detalles de orden.
- [x] Logs de inventario.
- [x] Facturas.
- [x] Estados de factura.
- [x] Pagos.
- [x] Metodos de pago.
- [x] Garantias.
- [x] Historial de kilometraje.
- [x] Usuarios.
- [x] Roles.
- [x] Auditoria.

## Seguridad

- [x] Login JWT implementado en `POST /api/Auth/login`.
- [x] Token incluye identificador, correo, nombre y rol.
- [x] Password hashing con PBKDF2.
- [x] Creacion de usuarios exige contrasena.
- [x] Reset de contrasena por Admin.
- [x] Autorizacion por roles implementada.
- [x] Politica `Admin`.
- [x] Politica `Recepcionista`.
- [x] Politica `Mecanico`.
- [x] Endpoints protegidos con `[Authorize]`.
- [x] Swagger configurado con Bearer JWT.
- [x] Rol del JWT normalizado para evitar errores por mayusculas/minusculas.

## Reglas de negocio implementadas

- [x] No permitir doble orden activa para el mismo vehiculo.
- [x] No permitir asignar un mecanico a dos ordenes activas.
- [x] No permitir crear o mover una cita para un vehiculo con orden activa.
- [x] No permitir citas duplicadas o cruzadas para el mismo vehiculo.
- [x] No permitir modificar ordenes en estado `Completada` o `Cancelada`.
- [x] No permitir asignar mecanico a orden terminal.
- [x] No permitir tareas, notas, detalles, historial o garantias sobre orden terminal.
- [x] No permitir facturar orden `Cancelada`.
- [x] Factura calcula mano de obra y repuestos usados.
- [x] Detalle de orden descuenta inventario.
- [x] Detalle de compra aumenta inventario.
- [x] Movimientos de inventario registran `LogInventario`.
- [x] No permitir actualizar factura con pagos confirmados.
- [x] No permitir modificar pago confirmado.
- [x] No permitir eliminar pago confirmado.
- [x] Borrado seguro para entidades con dependencias.

## Paginacion y filtros

- [x] `Cliente` con `pageNumber`, `pageSize`, `search` y `X-Total-Count`.
- [x] `Vehiculo` con paginacion, `clienteId`, `vin`, `placa` y `X-Total-Count`.
- [x] `OrdenServicio` con paginacion, `estadoId`, `vehiculoId`, `recepcionistaId`, rango de fechas y `X-Total-Count`.
- [x] `Repuesto` con paginacion, `categoriaId`, `search`, `stockMinimo`, `soloBajoStock` y `X-Total-Count`.

## Auditoria y trazabilidad

- [x] Tabla `Auditoria` implementada.
- [x] Auditoria manual disponible por API.
- [x] Auditoria automatica al crear `OrdenServicio`.
- [x] Auditoria automatica al crear `Factura`.
- [x] Auditoria automatica al crear `Pago`.
- [x] Auditoria automatica al actualizar `OrdenServicio`.
- [x] Auditoria automatica al actualizar `Factura`.
- [x] Auditoria automatica al actualizar `Pago`.
- [x] Consulta de auditoria con filtros por usuario, entidad, accion y rango de fechas.
- [x] `LogInventario` registra entradas y salidas de stock.

## Swagger y documentacion API

- [x] Swagger habilitado en Development.
- [x] Seguridad Bearer JWT configurada en Swagger.
- [x] Controladores principales documentan respuestas HTTP.
- [x] `AuthController` documentado.
- [x] `UsuarioController` documentado.
- [x] `ClienteController` documentado.
- [x] `VehiculoController` documentado.
- [x] `OrdenServicioController` documentado.
- [x] `RepuestoController` documentado.
- [x] `FacturaController` documentado.
- [x] `PagoController` documentado.
- [ ] Opcional: documentar respuestas HTTP en todos los catalogos restantes.

## Rate limiting

- [x] Rate limiting configurado con middleware nativo de ASP.NET Core.
- [x] Limite para `OrdenServicio`: 60 solicitudes por minuto.
- [x] Limite para `Repuesto`: 30 solicitudes por minuto.
- [x] Respuesta esperada al exceder limite: `429 Too Many Requests`.

## Datos semilla

- [x] Seeder idempotente al iniciar API.
- [x] Roles base.
- [x] Estados de orden base.
- [x] Estados de factura base.
- [x] Metodos de pago base.
- [x] Tipos de servicio base.
- [x] Categorias y unidades base.
- [x] Usuarios de prueba en ambiente Development.
- [x] Login probado con `admin.seed@autotaller.com`.

## Pruebas manuales realizadas

- [x] Login JWT devuelve token.
- [x] Swagger autoriza con token Bearer.
- [x] Admin puede consultar usuarios.
- [x] Recepcionista no puede consultar usuarios.
- [x] Mecanico no puede consultar clientes.
- [x] Recepcionista puede consultar clientes.
- [x] Creacion de rol responde `201`.
- [x] Validacion de rol vacio responde `400`.
- [x] Usuario con rol inexistente responde `404`.
- [x] Cliente creado correctamente.
- [x] Vehiculo creado correctamente.
- [x] Orden de servicio creada correctamente.
- [x] Bloqueo por doble orden activa responde `409`.
- [x] Bloqueo por mecanico ocupado responde `409`.
- [x] Bloqueo de cita por vehiculo con orden activa responde `409`.
- [x] Bloqueo de factura para orden cancelada responde `409`.
- [x] Compra y detalle de compra actualizan stock.
- [x] Detalle de orden descuenta stock.
- [x] Factura calcula totales.
- [x] Pago confirmado creado correctamente.
- [x] Auditoria registra acciones.
- [x] Paginacion devuelve header `X-Total-Count`.
- [x] Flujo completo final documentado en `docs/prueba-final-flujo.md`.

## Pendientes recomendados antes de entrega

- [x] Crear `README.md` final con instrucciones de instalacion, ejecucion y pruebas.
- [x] Probar flujo completo en la base actual del proyecto usando Swagger.
- [x] Prueba en base limpia marcada como opcional/no aplica para esta entrega, porque se conserva la base actual de trabajo.
- [x] Revisar que `appsettings.Development.json` no tenga secretos reales.
- [x] Confirmar cadena de conexion esperada para el entorno de entrega.
- [x] Ejecutar `dotnet build --no-restore` antes de entregar.
- [x] Ejecutar `dotnet ef database update -p .\Infrastructure -s .\Api` en la base final.

### Prueba segura en base limpia (opcional/no aplica)

Esta prueba queda como opcional para esta entrega. No se debe borrar ni modificar la base actual de trabajo solo para esta validacion.

Si en el futuro se quiere validar instalacion desde cero, usar este flujo:

1. Crear una base nueva, por ejemplo `AutoTallerManagerDb_Clean`.
2. Cambiar temporalmente `ConnectionStrings:Postgres` para apuntar a esa base.
3. Ejecutar:

```bash
dotnet ef database update -p .\Infrastructure -s .\Api
```

4. Levantar la API:

```bash
dotnet run --project .\Api
```

5. Entrar a Swagger y hacer login con:

```json
{
  "correo": "admin.seed@autotaller.com",
  "contrasena": "Admin123!"
}
```

6. Confirmar que los catalogos semilla existen y que `GET /api/Usuario` responde `200 OK` con token Admin.

## Pendientes opcionales de mejora

- [ ] Tests automatizados unitarios o de integracion.
- [ ] Documentacion HTTP completa para todos los catalogos.
- [ ] Rutas REST en plural, si se decide cambiar convencion publica.
- [ ] Versionado de API.
- [ ] Health checks.
- [ ] Logging estructurado.
- [ ] Politicas de CORS para frontend.
- [ ] Docker Compose para API + PostgreSQL.

## Estado general

Estado estimado para entrega academica o demostracion: `85% - 90%`.

Estado estimado para produccion profesional con pruebas automatizadas, observabilidad y despliegue completo: `70% - 75%`.
