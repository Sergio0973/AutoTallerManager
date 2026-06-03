import apiClient, { getTotalCount } from "../client"
import type { 
  Repuesto, 
  RepuestoCreate, 
  RepuestoUpdate, 
  RepuestoFilterParams,
  PaginatedResponse,
  CategoriaRepuesto,
  UnidadMedida,
  Ubicacion
} from "../types"

export const inventarioService = {
  /**
   * Obtener lista paginada de repuestos
   */
  async getAll(params?: RepuestoFilterParams): Promise<PaginatedResponse<Repuesto>> {
    const response = await apiClient.get<Repuesto[]>("/Repuesto", { params })
    return {
      data: response.data,
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener un repuesto por ID
   */
  async getById(id: number): Promise<Repuesto> {
    const response = await apiClient.get<Repuesto>(`/Repuesto/${id}`)
    return response.data
  },

  /**
   * Crear un nuevo repuesto
   */
  async create(data: RepuestoCreate): Promise<Repuesto> {
    const response = await apiClient.post<Repuesto>("/Repuesto", data)
    return response.data
  },

  /**
   * Actualizar un repuesto existente
   */
  async update(id: number, data: RepuestoUpdate): Promise<Repuesto> {
    const response = await apiClient.put<Repuesto>(`/Repuesto/${id}`, data)
    return response.data
  },

  /**
   * Eliminar un repuesto
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/Repuesto/${id}`)
  },

  /**
   * Obtener repuestos con bajo stock
   */
  async getLowStock(): Promise<Repuesto[]> {
    const response = await apiClient.get<Repuesto[]>("/Repuesto", {
      params: { soloBajoStock: true }
    })
    return response.data
  },

  /**
   * Obtener categorías de repuestos
   */
  async getCategorias(): Promise<CategoriaRepuesto[]> {
    const response = await apiClient.get<CategoriaRepuesto[]>("/CategoriaRepuesto")
    return response.data
  },

  /**
   * Obtener unidades de medida
   */
  async getUnidadesMedida(): Promise<UnidadMedida[]> {
    const response = await apiClient.get<UnidadMedida[]>("/UnidadMedida")
    return response.data
  },

  /**
   * Obtener ubicaciones
   */
  async getUbicaciones(): Promise<Ubicacion[]> {
    const response = await apiClient.get<Ubicacion[]>("/Ubicacion")
    return response.data
  },
}
