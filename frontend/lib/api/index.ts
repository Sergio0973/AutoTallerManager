// Re-export all services
export { authService } from "./services/auth.service"
export { clienteService } from "./services/cliente.service"
export { vehiculoService } from "./services/vehiculo.service"
export { ordenService } from "./services/orden.service"
export { inventarioService } from "./services/inventario.service"
export { facturacionService } from "./services/facturacion.service"
export { citaService } from "./services/cita.service"
export { auditoriaService } from "./services/auditoria.service"
export { usuarioService } from "./services/usuario.service"
export { ubicacionService } from "./services/ubicacion.service"

// Re-export types
export * from "./types"

// Re-export client
export { default as apiClient } from "./client"
