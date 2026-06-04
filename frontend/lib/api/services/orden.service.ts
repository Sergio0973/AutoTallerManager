import apiClient, { getTotalCount } from "../client"
import { AxiosError } from "axios"
import type { 
  OrdenServicio, 
  OrdenServicioCreate, 
  OrdenServicioUpdate, 
  OrdenFilterParams,
  PaginatedResponse,
  EstadoOrden,
  DetalleOrden,
  DetalleOrdenCreate,
  OrdenMecanico,
  TareaMecanico,
  TareaMecanicoCreate,
  TipoServicio
} from "../types"

type OrdenServicioDto = {
  id: number
  vehiculoId: number
  recepcionistaId?: number
  estadoId: number
  citaId?: number | null
  kilometrajeIngreso: number
  fechaIngreso: string
  fechaEstimada?: string
  fechaEntregaReal?: string | null
  observaciones?: string | null
}

const fallbackTiposServicio: TipoServicio[] = [
  { id: 1, nombre: "Diagnostico", descripcion: "Revision inicial para identificar fallas." },
  { id: 2, nombre: "Mantenimiento preventivo", descripcion: "Servicio programado para prevenir fallas." },
  { id: 3, nombre: "Reparacion", descripcion: "Correccion de fallas detectadas en el vehiculo." },
]

const fallbackEstadosOrden: EstadoOrden[] = [
  { id: 1, nombre: "RECIBIDA", descripcion: "Orden creada y pendiente de diagnostico" },
  { id: 2, nombre: "DIAGNOSTICO", descripcion: "Vehiculo en revision tecnica" },
  { id: 3, nombre: "REPARACION", descripcion: "Trabajo mecanico en ejecucion" },
  { id: 4, nombre: "LISTA", descripcion: "Servicio finalizado y pendiente de entrega" },
  { id: 5, nombre: "ENTREGADA", descripcion: "Vehiculo entregado al cliente" },
  { id: 8, nombre: "Cancelada", descripcion: "Orden cancelada y sin posibilidad de facturacion" },
]

type OrdenServicioUpdatePayload = {
  vehiculoId: number
  recepcionistaId: number
  estadoId: number
  citaId?: number | null
  kilometrajeIngreso: number
  fechaEstimada?: string
  fechaEntregaReal?: string | null
  observaciones?: string
}

const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (error instanceof AxiosError) {
    const data = error.response?.data

    if (error.response?.status === 403) {
      return "No tienes permiso para realizar esta accion. Cierra sesion e inicia sesion de nuevo con el rol correcto."
    }

    if (data && typeof data === "object" && "message" in data && typeof data.message === "string") {
      return data.message
    }

    if (typeof data === "string" && data.trim()) {
      return data
    }
  }

  if (error instanceof Error && error.message) {
    return error.message
  }

  return fallback
}

const mapOrden = (orden: OrdenServicioDto): OrdenServicio => ({
  id: orden.id,
  fechaIngreso: orden.fechaIngreso,
  fechaEstimada: orden.fechaEstimada,
  fechaEstimadaEntrega: orden.fechaEstimada,
  fechaEntregaReal: orden.fechaEntregaReal ?? undefined,
  fechaEntrega: orden.fechaEntregaReal ?? undefined,
  observaciones: orden.observaciones ?? "",
  descripcionProblema: orden.observaciones ?? "",
  kilometrajeIngreso: orden.kilometrajeIngreso,
  vehiculoId: orden.vehiculoId,
  estadoId: orden.estadoId,
  citaId: orden.citaId ?? undefined,
})

const mapOrdenCreatePayload = (data: OrdenServicioCreate) => ({
  vehiculoId: data.vehiculoId,
  recepcionistaId: data.recepcionistaId,
  estadoId: data.estadoId ?? 1,
  citaId: data.citaId ?? null,
  kilometrajeIngreso: data.kilometrajeIngreso,
  fechaIngreso: data.fechaIngreso,
  fechaEstimada: data.fechaEstimada ?? data.fechaEstimadaEntrega,
  observaciones: data.observaciones ?? data.descripcionProblema,
})

export const ordenService = {
  /**
   * Obtener lista paginada de órdenes de servicio
   */
  async getAll(params?: OrdenFilterParams): Promise<PaginatedResponse<OrdenServicio>> {
    const response = await apiClient.get<OrdenServicioDto[]>("/OrdenServicio", { params })
    return {
      data: response.data.map(mapOrden),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener una orden por ID
   */
  async getById(id: number): Promise<OrdenServicio> {
    const response = await apiClient.get<OrdenServicioDto>(`/OrdenServicio/${id}`)
    return mapOrden(response.data)
  },

  /**
   * Crear una nueva orden de servicio
   */
  async create(data: OrdenServicioCreate): Promise<OrdenServicio> {
    try {
      const response = await apiClient.post<OrdenServicioDto>("/OrdenServicio", mapOrdenCreatePayload(data))
      return mapOrden(response.data)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo crear la orden."))
    }
  },

  /**
   * Actualizar una orden existente
   */
  async update(id: number, data: OrdenServicioUpdate): Promise<OrdenServicio> {
    try {
      await apiClient.put(`/OrdenServicio/${id}`, data)
      return this.getById(id)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo actualizar la orden."))
    }
  },

  async updateFull(id: number, data: OrdenServicioUpdatePayload): Promise<OrdenServicio> {
    try {
      await apiClient.put(`/OrdenServicio/${id}`, data)
      return this.getById(id)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo actualizar la orden."))
    }
  },

  /**
   * Eliminar una orden
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/OrdenServicio/${id}`)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo eliminar la orden."))
    }
  },

  /**
   * Obtener todos los estados de orden
   */
  async getEstados(): Promise<EstadoOrden[]> {
    try {
      const response = await apiClient.get<EstadoOrden[]>("/EstadoOrden")
      return response.data.length > 0 ? response.data : fallbackEstadosOrden
    } catch {
      return fallbackEstadosOrden
    }
  },

  /**
   * Obtener tipos de servicio
   */
  async getTiposServicio(): Promise<TipoServicio[]> {
    try {
      const response = await apiClient.get<TipoServicio[]>("/TipoServicio")
      return response.data.length > 0 ? response.data : fallbackTiposServicio
    } catch {
      return fallbackTiposServicio
    }
  },

  // Detalles de orden
  async getDetallesByOrden(ordenId: number): Promise<DetalleOrden[]> {
    const response = await apiClient.get<DetalleOrden[]>("/DetalleOrden", { params: { ordenId } })
    return response.data
  },

  async addDetalle(data: DetalleOrdenCreate): Promise<DetalleOrden> {
    const response = await apiClient.post<DetalleOrden>("/DetalleOrden", data)
    return response.data
  },

  async deleteDetalle(id: number): Promise<void> {
    await apiClient.delete(`/DetalleOrden/${id}`)
  },

  // Asignación de mecánicos
  async asignarMecanico(ordenId: number, mecanicoId: number): Promise<OrdenMecanico> {
    try {
      const response = await apiClient.post<OrdenMecanico>("/OrdenMecanico", {
        ordenId,
        mecanicoId,
        fechaAsignacion: new Date().toISOString().split("T")[0],
      })
      return response.data
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo asignar el mecanico."))
    }
  },

  async removerMecanico(id: number): Promise<void> {
    await apiClient.delete(`/OrdenMecanico/${id}`)
  },

  // Tareas de mecánico
  async getTareasByOrden(ordenId: number): Promise<TareaMecanico[]> {
    const response = await apiClient.get<TareaMecanico[]>("/TareaMecanico", { params: { ordenId } })
    return response.data
  },

  async addTarea(data: TareaMecanicoCreate): Promise<TareaMecanico> {
    const response = await apiClient.post<TareaMecanico>("/TareaMecanico", data)
    return response.data
  },

  async completarTarea(id: number): Promise<TareaMecanico> {
    const response = await apiClient.patch<TareaMecanico>(`/TareaMecanico/${id}/completar`)
    return response.data
  },

  async deleteTarea(id: number): Promise<void> {
    await apiClient.delete(`/TareaMecanico/${id}`)
  },
}
