# Diseno de refactorizacion para Project-Taller-Mecanico

## Objetivo

Refactorizar `Project-Taller-Mecanico` para que quede alineado con el estilo arquitectonico de `SST-PESV-BACKEND`, manteniendo separacion clara entre `Api`, `Application`, `Domain` e `Infrastructure`, y dejando una base funcional, compilable y extensible.

`SST-PESV-BACKEND` se usa solo como referencia de patrones. No se modifica.

## Estado actual

- `Domain` contiene entidades y value objects suficientes para un modelo inicial del taller.
- `Application` solo expone abstracciones de repositorio y `IUnitOfWork`; no tiene casos de uso ni pipeline de validacion.
- `Infrastructure` tiene `DbContext`, configuraciones y repositorios, pero actualmente no compila por desalineacion entre entidad `Auditoria` y su configuracion EF.
- `Api` sigue casi el template inicial de ASP.NET y no expone una superficie CRUD consistente con la arquitectura de referencia.

## Arquitectura objetivo

La estructura se llevara a un modelo por responsabilidad, respetando las capas existentes:

- `Domain`: entidades, value objects y reglas de negocio.
- `Application`: abstracciones, comandos/casos de uso, validadores y configuracion MediatR.
- `Infrastructure`: persistencia EF Core, configuraciones, repositorios y `UnitOfWork`.
- `Api`: DTOs, mapeos, controladores y bootstrap de la aplicacion.

No se hara una reescritura completa del dominio. Se trabajara sobre la base existente y se acomodaran los componentes necesarios para que el sistema funcione correctamente.

## Alcance funcional inicial

Se dejara implementado el flujo base para los modulos principales ya presentes en el proyecto:

- `Clientes`
- `Vehiculos`
- `OrdenesServicio`
- `Repuestos`
- `Facturas`
- `Usuarios`

Para estos modulos se busca:

- repositorios coherentes con el `DbContext`
- `UnitOfWork` estable
- casos de uso de creacion y actualizacion en `Application`
- controladores CRUD base en `Api`
- mapeos DTO <-> command/entity

## Estrategia de implementacion

1. Corregir compilacion y consistencia de `Infrastructure`.
2. Incorporar en `Application` el mismo patron base del backend de referencia:
   - `DependencyInjection`
   - MediatR
   - FluentValidation
   - `ValidationBehavior`
3. Crear casos de uso por modulo empezando por `Clientes`, que servira como patron replicable.
4. Ajustar `Api` para usar `ISender`, `IUnitOfWork` y DTOs de respuesta/peticion.
5. Verificar compilacion completa y arranque de la API.

## Decisiones tecnicas

- Se mantendra EF Core con PostgreSQL.
- Se preservara la separacion actual por proyectos (`Api`, `Application`, `Domain`, `Infrastructure`).
- Se evitara mover masivamente todas las carpetas fisicas del dominio en esta primera pasada; el objetivo es consistencia funcional y arquitectonica, no churn innecesario.
- El patron de referencia a copiar sera principalmente el de `Create/Update` con `MediatR`, validadores y controladores del workspace `SST-PESV-BACKEND`.

## Riesgos controlados

- Algunas entidades tienen muchas relaciones y value objects; por eso la refactorizacion se hara incremental.
- Puede haber mas configuraciones EF desalineadas ademas de `Auditoria`.
- No existe repositorio git inicializado dentro de `Project-Taller-Mecanico`, por lo que esta fase no incluira commit automatico del diseno.

## Criterios de cierre

La refactorizacion se considerara aceptable cuando:

- `dotnet build` termine sin errores.
- `Api` arranque con su configuracion de dependencias completa.
- Exista un patron claro y consistente para al menos los modulos base del sistema.
- La estructura resultante quede preparada para seguir ampliando features con el mismo estilo del backend de referencia.
