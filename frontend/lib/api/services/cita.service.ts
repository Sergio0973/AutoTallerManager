import apiClient, { getTotalCount } from "../client"
import type { 
  Cita, 
  CitaCreate, 
  PaginatedResponse,
  PaginationParams
} from "../types"

export const citaService = {
  /**
   * Obtener lista paginada de citas
   */
  async getAll(params?: PaginationParams): Promise<PaginatedResponse<Cita>> {
    const response = await apiClient.get<Cita[]>("/Cita", { params })
    return {
      data: response.data,
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener una cita por ID
   */
  async getById(id: number): Promise<Cita> {
    const response = await apiClient.get<Cita>(`/Cita/${id}`)
    return response.data
  },

  /**
   * Crear una nueva cita
   */
  async create(data: CitaCreate): Promise<Cita> {
    const response = await apiClient.post<Cita>("/Cita", data)
    return response.data
  },

  /**
   * Actualizar una cita
   */
  async update(id: number, data: Partial<CitaCreate>): Promise<Cita> {
    const response = await apiClient.put<Cita>(`/Cita/${id}`, data)
    return response.data
  },

  /**
   * Cancelar una cita
   */
  async cancel(id: number): Promise<void> {
    await apiClient.patch(`/Cita/${id}/cancelar`)
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
    const response = await apiClient.get<Cita[]>("/Cita", {
      params: { fecha: today }
    })
    return response.data
  },
}
