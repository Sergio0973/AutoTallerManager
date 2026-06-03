import apiClient, { getTotalCount } from "../client"
import type { 
  Vehiculo, 
  VehiculoCreate, 
  VehiculoUpdate, 
  VehiculoFilterParams,
  PaginatedResponse,
  Marca,
  Modelo
} from "../types"

type VehiculoDto = {
  id: number
  clienteId: number
  modeloId: number
  vin?: string
  anio: number
  placa: string
  color?: string
  activo?: boolean
}

const mapVehiculo = (vehiculo: VehiculoDto): Vehiculo => ({
  id: vehiculo.id,
  clienteId: vehiculo.clienteId,
  modeloId: vehiculo.modeloId,
  marcaId: 0,
  placa: vehiculo.placa,
  vin: vehiculo.vin,
  anio: vehiculo.anio,
  color: vehiculo.color,
  kilometraje: 0,
  fechaRegistro: "",
  activo: vehiculo.activo,
})

const mapVehiculoPayload = (data: VehiculoCreate | VehiculoUpdate) => ({
  clienteId: data.clienteId,
  modeloId: data.modeloId,
  vin: data.vin,
  anio: data.anio,
  placa: data.placa,
  color: data.color,
})

export const vehiculoService = {
  /**
   * Obtener lista paginada de vehículos
   */
  async getAll(params?: VehiculoFilterParams): Promise<PaginatedResponse<Vehiculo>> {
    const response = await apiClient.get<VehiculoDto[]>("/Vehiculo", { params })
    return {
      data: response.data.map(mapVehiculo),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener un vehículo por ID
   */
  async getById(id: number): Promise<Vehiculo> {
    const response = await apiClient.get<VehiculoDto>(`/Vehiculo/${id}`)
    return mapVehiculo(response.data)
  },

  /**
   * Crear un nuevo vehículo
   */
  async create(data: VehiculoCreate): Promise<Vehiculo> {
    const response = await apiClient.post<VehiculoDto>("/Vehiculo", mapVehiculoPayload(data))
    return mapVehiculo(response.data)
  },

  /**
   * Actualizar un vehículo existente
   */
  async update(id: number, data: VehiculoUpdate): Promise<Vehiculo> {
    const response = await apiClient.put<VehiculoDto>(`/Vehiculo/${id}`, mapVehiculoPayload(data))
    return mapVehiculo(response.data)
  },

  /**
   * Eliminar un vehículo
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/Vehiculo/${id}`)
  },

  /**
   * Obtener todas las marcas
   */
  async getMarcas(): Promise<Marca[]> {
    const response = await apiClient.get<Marca[]>("/MarcaVehiculo")
    return response.data
  },

  /**
   * Obtener modelos por marca
   */
  async getModelosByMarca(marcaId: number): Promise<Modelo[]> {
    const response = await apiClient.get<Modelo[]>("/ModeloVehiculo")
    return response.data.filter((modelo) => modelo.marcaId === marcaId)
  },

  async getModelos(): Promise<Modelo[]> {
    const response = await apiClient.get<Modelo[]>("/ModeloVehiculo")
    return response.data
  },
}
