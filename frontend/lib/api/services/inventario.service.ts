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

type RepuestoDto = {
  id: number
  categoriaId: number
  unidadId: number
  codigo: string
  descripcion: string
  stockActual: number
  stockMinimo: number
  precioUnitario: number
  activo: boolean
}

const mapRepuesto = (repuesto: RepuestoDto): Repuesto => ({
  id: repuesto.id,
  categoriaId: repuesto.categoriaId,
  unidadId: repuesto.unidadId,
  unidadMedidaId: repuesto.unidadId,
  codigo: repuesto.codigo,
  nombre: repuesto.descripcion,
  descripcion: repuesto.descripcion,
  precioUnitario: repuesto.precioUnitario,
  precioVenta: repuesto.precioUnitario,
  precioCosto: repuesto.precioUnitario,
  stockActual: repuesto.stockActual,
  stockMinimo: repuesto.stockMinimo,
  activo: repuesto.activo,
})

const mapRepuestoPayload = (data: RepuestoCreate | RepuestoUpdate) => ({
  categoriaId: data.categoriaId,
  unidadId: data.unidadId ?? data.unidadMedidaId,
  codigo: data.codigo,
  descripcion: data.descripcion || data.nombre || "",
  stockActual: data.stockActual,
  stockMinimo: data.stockMinimo,
  precioUnitario: data.precioUnitario ?? data.precioVenta ?? data.precioCosto,
})

export const inventarioService = {
  /**
   * Obtener lista paginada de repuestos
   */
  async getAll(params?: RepuestoFilterParams): Promise<PaginatedResponse<Repuesto>> {
    const response = await apiClient.get<RepuestoDto[]>("/Repuesto", { params })
    return {
      data: response.data.map(mapRepuesto),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener un repuesto por ID
   */
  async getById(id: number): Promise<Repuesto> {
    const response = await apiClient.get<RepuestoDto>(`/Repuesto/${id}`)
    return mapRepuesto(response.data)
  },

  /**
   * Crear un nuevo repuesto
   */
  async create(data: RepuestoCreate): Promise<Repuesto> {
    const response = await apiClient.post<RepuestoDto>("/Repuesto", mapRepuestoPayload(data))
    return mapRepuesto(response.data)
  },

  /**
   * Actualizar un repuesto existente
   */
  async update(id: number, data: RepuestoUpdate): Promise<Repuesto> {
    const response = await apiClient.put<RepuestoDto>(`/Repuesto/${id}`, mapRepuestoPayload(data))
    return mapRepuesto(response.data)
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
    const response = await apiClient.get<RepuestoDto[]>("/Repuesto", {
      params: { soloBajoStock: true }
    })
    return response.data.map(mapRepuesto)
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
