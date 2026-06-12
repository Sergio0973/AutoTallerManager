// Tipos para el módulo de Autenticación
export interface LoginRequest {
  correo: string
  contrasena: string
}

export interface LoginResponse {
  token: string
  expiraEn: string
  usuario: Usuario
}

export interface Usuario {
  id: number
  rolId?: number
  correo: string
  nombre: string
  nombreCompleto?: string
  rol?: string | Rol
  activo?: boolean
  fechaCreacion?: string
}

export interface Rol {
  id: number
  nombre: string // Admin, Recepcionista, Mecanico
  descripcion?: string
}

export interface UsuarioCreate {
  rolId: number
  correo: string
  nombre: string
  contrasena: string
}

export interface UsuarioUpdate {
  rolId: number
  correo: string
  nombre: string
}

export interface UsuarioPasswordReset {
  nuevaContrasena: string
}

export interface RolCreate {
  nombre: string
  descripcion: string
}

export interface RolUpdate extends RolCreate {}

export interface Auditoria {
  id: number
  usuarioId: number
  entidad: string
  entidadId: number
  tipoAccion: string
  datosAnteriores?: string | null
  datosNuevos?: string | null
  ipOrigen: string
  fecha: string
}

export interface AuditoriaFilterParams {
  usuarioId?: number
  entidad?: string
  tipoAccion?: string
  desde?: string
  hasta?: string
}

export interface Pais {
  id: number
  nombre: string
  codigo: string
}

export interface PaisCreate {
  nombre: string
  codigo: string
}

export interface Departamento {
  id: number
  paisId: number
  nombre: string
}

export interface DepartamentoCreate {
  paisId: number
  nombre: string
}

export interface Ciudad {
  id: number
  departamentoId: number
  nombre: string
}

export interface CiudadCreate {
  departamentoId: number
  nombre: string
}

// Tipos para Clientes
export interface Cliente {
  id: number
  nombres?: string
  apellidos?: string
  documento?: string
  nombre: string
  apellido: string
  correo: string
  correoId?: number
  telefono: string
  telefonoId?: number
  direccion?: string
  direccionId?: number
  ciudadId?: number
  fechaRegistro: string
  activo: boolean
}

export interface ClienteCreate {
  nombre: string
  apellido: string
  documento: string
  correo?: string
  telefono?: string
  direccion?: string
  ciudadId?: number
}

export interface ClienteUpdate extends ClienteCreate {
  activo?: boolean
}

// Tipos para Vehículos
export interface Vehiculo {
  id: number
  placa: string
  vin?: string
  color?: string
  anio: number
  kilometraje: number
  clienteId: number
  cliente?: Cliente
  marcaId: number
  marca?: Marca
  modeloId: number
  modelo?: Modelo
  fechaRegistro: string
  activo?: boolean
}

export interface VehiculoCreate {
  placa: string
  vin?: string
  color?: string
  anio: number
  kilometraje: number
  clienteId: number
  marcaId: number
  modeloId: number
}

export interface VehiculoUpdate extends VehiculoCreate {}

export interface Marca {
  id: number
  nombre: string
}

export interface Modelo {
  id: number
  nombre: string
  marcaId: number
  anioDesde?: number
  anioHasta?: number
}

// Tipos para Citas
export interface Cita {
  id: number
  vehiculoId: number
  recepcionistaId?: number
  tipoServicioId?: number
  fechaCita?: string
  horaInicio?: string
  horaFin?: string
  observaciones?: string
  fechaHora: string
  motivo: string
  notas?: string
  vehiculo?: Vehiculo
  estado: string
  fechaCreacion: string
}

export interface CitaCreate {
  vehiculoId: number
  recepcionistaId?: number
  tipoServicioId?: number
  fechaCita?: string
  horaInicio?: string
  horaFin?: string
  estado?: string
  observaciones?: string
  fechaHora?: string
  motivo?: string
  notas?: string
}

// Tipos para Órdenes de Servicio
export interface OrdenServicio {
  id: number
  fechaIngreso: string
  fechaEstimada?: string
  fechaEstimadaEntrega?: string
  fechaEntregaReal?: string
  fechaEntrega?: string
  observaciones?: string
  descripcionProblema: string
  diagnostico?: string
  kilometrajeIngreso: number
  vehiculoId: number
  vehiculo?: Vehiculo
  estadoId: number
  estado?: EstadoOrden
  citaId?: number
  detalles?: DetalleOrden[]
  mecanicos?: OrdenMecanico[]
  tiposServicio?: OrdenTipoServicio[]
}

export interface OrdenServicioCreate {
  fechaIngreso: string
  fechaEstimada?: string
  fechaEstimadaEntrega?: string
  observaciones?: string
  descripcionProblema: string
  kilometrajeIngreso: number
  vehiculoId: number
  recepcionistaId?: number
  estadoId?: number
  citaId?: number
}

export interface OrdenServicioUpdate {
  fechaEstimadaEntrega?: string
  diagnostico?: string
  estadoId?: number
}

export interface EstadoOrden {
  id: number
  nombre: string // Pendiente, En proceso, Completada, Cancelada
  descripcion?: string
}

export interface OrdenMecanico {
  id: number
  ordenId: number
  mecanicoId: number
  mecanico?: Usuario
  fechaAsignacion: string
}

export interface OrdenTipoServicio {
  id: number
  ordenId: number
  tipoServicioId: number
  tipoServicio?: TipoServicio
}

export interface TipoServicio {
  id: number
  nombre: string // Diagnóstico, Mantenimiento preventivo, Reparación
  descripcion?: string
}

export interface DetalleOrden {
  id: number
  ordenId: number
  repuestoId: number
  repuesto?: Repuesto
  cantidad: number
  precioSnapshot: number
  subtotal: number
}

export interface DetalleOrdenCreate {
  ordenId: number
  repuestoId: number
  usuarioId: number
  cantidad: number
  precioSnapshot: number
}

// Tipos para Repuestos/Inventario
export interface Repuesto {
  id: number
  categoriaId: number
  unidadId?: number
  unidadIdMedida?: number
  codigo: string
  descripcion?: string
  nombre: string
  precioUnitario?: number
  precioVenta: number
  precioCosto: number
  stockActual: number
  stockMinimo: number
  categoria?: CategoriaRepuesto
  unidadMedidaId: number
  unidadMedida?: UnidadMedida
  ubicacionId?: number
  ubicacion?: Ubicacion
  activo: boolean
}

export interface RepuestoCreate {
  categoriaId: number
  unidadId?: number
  unidadMedidaId?: number
  codigo: string
  descripcion?: string
  nombre?: string
  precioUnitario?: number
  precioVenta?: number
  precioCosto: number
  stockActual: number
  stockMinimo: number
  ubicacionId?: number
}

export interface RepuestoUpdate extends RepuestoCreate {
  activo?: boolean
}

export interface CategoriaRepuesto {
  id: number
  nombre: string // Encendido, Frenos, Filtros
}

export interface UnidadMedida {
  id: number
  nombre: string // Unidad, Litro
}

export interface Ubicacion {
  id: number
  nombre: string
  descripcion?: string
}

export interface Proveedor {
  id: number
  nombre: string
  nit: string
  telefono: string
  correo: string
  ciudadId: number
  activo: boolean
}

export interface ProveedorCreate {
  nombre: string
  nit: string
  telefono: string
  correo: string
  ciudadId: number
}

export interface Compra {
  id: number
  proveedorId: number
  usuarioId: number
  fechaCompra: string
  total: number
  estado: string
  observaciones?: string
}

export interface CompraCreate {
  proveedorId: number
  usuarioId: number
  fechaCompra: string
  estado: string
  observaciones?: string
}

export interface DetalleCompra {
  id: number
  compraId: number
  repuestoId: number
  cantidad: number
  precioUnitario: number
  subtotal: number
}

export interface DetalleCompraCreate {
  compraId: number
  repuestoId: number
  cantidad: number
  precioUnitario: number
}

// Tipos para Facturación
export interface Factura {
  id: number
  numero?: string
  ordenId: number
  estadoFacturaId?: number
  usuarioId?: number
  manoDeObra?: number
  costoRepuestos?: number
  descuento?: number
  impuestoPct?: number
  fechaEmision: string
  subtotal: number
  impuesto?: number
  total: number
  orden?: OrdenServicio
  estadoId?: number
  estado?: EstadoFactura
  pagos?: Pago[]
  observaciones?: string
}

export interface FacturaCreate {
  ordenId: number
  estadoFacturaId?: number
  usuarioId?: number
  descuento?: number
  impuestoPct?: number
  fechaEmision?: string
  observaciones?: string
}

export interface EstadoFactura {
  id: number
  nombre: string // Emitida, Pagada, Anulada
}

export interface Pago {
  id: number
  facturaId: number
  metodoPagoId: number
  monto: number
  fechaPago: string
  referencia?: string
  metodoPago?: MetodoPago
  confirmado: boolean
  estado?: string
}

export interface PagoCreate {
  facturaId: number
  metodoPagoId: number
  monto: number
  referencia?: string
  estado?: string
}

export interface MetodoPago {
  id: number
  nombre: string // Efectivo, Tarjeta, Transferencia
}

// Tipos para tareas de mecánicos
export interface TareaMecanico {
  id: number
  ordenId: number
  mecanicoId: number
  tipoServicioId: number
  descripcion: string
  horasTrabajadas: number
  costoHora: number
  costoTotal: number
  estado: string
  fechaInicio?: string
  fechaFin?: string
}

export interface TareaMecanicoCreate {
  ordenId: number
  mecanicoId: number
  tipoServicioId: number
  descripcion: string
  horasTrabajadas: number
  costoHora: number
  estado: string
  fechaInicio?: string
  fechaFin?: string
}

// Tipos para parámetros de búsqueda/filtrado
export interface PaginationParams {
  pageNumber?: number
  pageSize?: number
  search?: string
}

export interface PaginatedResponse<T> {
  data: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
}

export interface ClienteFilterParams extends PaginationParams {}

export interface VehiculoFilterParams extends PaginationParams {
  clienteId?: number
}

export interface OrdenFilterParams extends PaginationParams {
  estadoId?: number
  vehiculoId?: number
  mecanicoId?: number
}

export interface RepuestoFilterParams extends PaginationParams {
  categoriaId?: number
  soloBajoStock?: boolean
}
