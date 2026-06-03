import apiClient, { getTotalCount } from "../client"
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
    const response = await apiClient.post<OrdenServicioDto>("/OrdenServicio", mapOrdenCreatePayload(data))
    return mapOrden(response.data)
  },

  /**
   * Actualizar una orden existente
   */
  async update(id: number, data: OrdenServicioUpdate): Promise<OrdenServicio> {
    const response = await apiClient.put<OrdenServicio>(`/OrdenServicio/${id}`, data)
    return response.data
  },

  /**
   * Eliminar una orden
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/OrdenServicio/${id}`)
  },

  /**
   * Obtener todos los estados de orden
   */
  async getEstados(): Promise<EstadoOrden[]> {
    const response = await apiClient.get<EstadoOrden[]>("/EstadoOrden")
    return response.data
  },

  /**
   * Obtener tipos de servicio
   */
  async getTiposServicio(): Promise<TipoServicio[]> {
    const response = await apiClient.get<TipoServicio[]>("/TipoServicio")
    return response.data
  },

  // Detalles de orden
  async addDetalle(data: DetalleOrdenCreate): Promise<DetalleOrden> {
    const response = await apiClient.post<DetalleOrden>("/DetalleOrden", data)
    return response.data
  },

  async deleteDetalle(id: number): Promise<void> {
    await apiClient.delete(`/DetalleOrden/${id}`)
  },

  // Asignación de mecánicos
  async asignarMecanico(ordenId: number, mecanicoId: number): Promise<OrdenMecanico> {
    const response = await apiClient.post<OrdenMecanico>("/OrdenMecanico", {
      ordenId,
      mecanicoId,
      fechaAsignacion: new Date().toISOString().split("T")[0],
    })
    return response.data
  },

  async removerMecanico(id: number): Promise<void> {
    await apiClient.delete(`/OrdenMecanico/${id}`)
  },

  // Tareas de mecánico
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
