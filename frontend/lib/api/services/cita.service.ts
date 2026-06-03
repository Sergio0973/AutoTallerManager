import apiClient, { getTotalCount } from "../client"
import type { 
  Cita, 
  CitaCreate, 
  PaginatedResponse,
  PaginationParams
} from "../types"

type CitaDto = {
  id: number
  vehiculoId: number
  recepcionistaId: number
  tipoServicioId: number
  fechaCita: string
  horaInicio: string
  horaFin: string
  estado: string
  observaciones?: string | null
}

const mapCita = (cita: CitaDto): Cita => ({
  id: cita.id,
  vehiculoId: cita.vehiculoId,
  recepcionistaId: cita.recepcionistaId,
  tipoServicioId: cita.tipoServicioId,
  fechaCita: cita.fechaCita,
  horaInicio: cita.horaInicio,
  horaFin: cita.horaFin,
  estado: cita.estado,
  observaciones: cita.observaciones ?? "",
  fechaHora: `${cita.fechaCita}T${cita.horaInicio}`,
  motivo: cita.observaciones ?? "Cita de servicio",
  notas: cita.observaciones ?? "",
  fechaCreacion: cita.fechaCita,
})

const mapCitaPayload = (data: CitaCreate | Partial<CitaCreate>) => ({
  vehiculoId: data.vehiculoId,
  recepcionistaId: data.recepcionistaId,
  tipoServicioId: data.tipoServicioId,
  fechaCita: data.fechaCita ?? data.fechaHora?.split("T")[0],
  horaInicio: data.horaInicio ?? data.fechaHora?.split("T")[1]?.slice(0, 5) ?? "08:00",
  horaFin: data.horaFin ?? "09:00",
  estado: data.estado ?? "Programada",
  observaciones: data.observaciones ?? data.notas ?? data.motivo,
})

export const citaService = {
  /**
   * Obtener lista paginada de citas
   */
  async getAll(params?: PaginationParams): Promise<PaginatedResponse<Cita>> {
    const response = await apiClient.get<CitaDto[]>("/Cita", { params })
    return {
      data: response.data.map(mapCita),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener una cita por ID
   */
  async getById(id: number): Promise<Cita> {
    const response = await apiClient.get<CitaDto>(`/Cita/${id}`)
    return mapCita(response.data)
  },

  /**
   * Crear una nueva cita
   */
  async create(data: CitaCreate): Promise<Cita> {
    const response = await apiClient.post<CitaDto>("/Cita", mapCitaPayload(data))
    return mapCita(response.data)
  },

  /**
   * Actualizar una cita
   */
  async update(id: number, data: Partial<CitaCreate>): Promise<Cita> {
    await apiClient.put(`/Cita/${id}`, mapCitaPayload(data))
    return this.getById(id)
  },

  /**
   * Cancelar una cita
   */
  async cancel(id: number): Promise<void> {
    const cita = await this.getById(id)
    await this.update(id, { ...cita, estado: "Cancelada" })
  },

  /**
   * Eliminar una cita
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/Cita/${id}`)
  },

  /**
   * Obtener citas del día
   */
  async getToday(): Promise<Cita[]> {
    const today = new Date().toISOString().split("T")[0]
    const response = await apiClient.get<CitaDto[]>("/Cita", {
      params: { fecha: today }
    })
    return response.data.map(mapCita)
  },
}
