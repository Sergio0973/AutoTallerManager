import apiClient, { getTotalCount } from "../client"
import { AxiosError } from "axios"
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

const fallbackMarcas: Marca[] = [
  { id: 1, nombre: "BASE" },
]

const fallbackModelos: Modelo[] = [
  { id: 1, marcaId: 1, nombre: "BASE", anioDesde: 2000, anioHasta: 2035 },
  { id: 2, marcaId: 1, nombre: "Corolla", anioDesde: 2020, anioHasta: 2026 },
]

const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (error instanceof AxiosError) {
    const data = error.response?.data

    if (data && typeof data === "object" && "message" in data && typeof data.message === "string") {
      if ("errors" in data && Array.isArray(data.errors) && data.errors.length > 0) {
        const details = data.errors
          .map((error) => {
            if (error && typeof error === "object" && "errorMessage" in error) {
              return String(error.errorMessage)
            }

            return ""
          })
          .filter(Boolean)
          .join(" ")

        return details ? `${data.message} ${details}` : data.message
      }

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
    try {
      const response = await apiClient.post<VehiculoDto>("/Vehiculo", mapVehiculoPayload(data))
      return mapVehiculo(response.data)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo crear el vehiculo."))
    }
  },

  /**
   * Actualizar un vehículo existente
   */
  async update(id: number, data: VehiculoUpdate): Promise<Vehiculo> {
    try {
      await apiClient.put(`/Vehiculo/${id}`, mapVehiculoPayload(data))
      return this.getById(id)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo actualizar el vehiculo."))
    }
  },

  /**
   * Eliminar un vehículo
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/Vehiculo/${id}`)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo eliminar el vehiculo."))
    }
  },

  /**
   * Obtener todas las marcas
   */
  async getMarcas(): Promise<Marca[]> {
    try {
      const response = await apiClient.get<Marca[]>("/MarcaVehiculo")
      return response.data.length > 0 ? response.data : fallbackMarcas
    } catch {
      return fallbackMarcas
    }
  },

  /**
   * Obtener modelos por marca
   */
  async getModelosByMarca(marcaId: number): Promise<Modelo[]> {
    const modelos = await this.getModelos()
    return modelos.filter((modelo) => modelo.marcaId === marcaId)
  },

  async getModelos(): Promise<Modelo[]> {
    try {
      const response = await apiClient.get<Modelo[]>("/ModeloVehiculo")
      return response.data.length > 0 ? response.data : fallbackModelos
    } catch {
      return fallbackModelos
    }
  },
}
