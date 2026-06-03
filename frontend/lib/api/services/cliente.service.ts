import apiClient, { getTotalCount } from "../client"
import type { 
  Cliente, 
  ClienteCreate, 
  ClienteUpdate, 
  ClienteFilterParams,
  PaginatedResponse 
} from "../types"

type ClienteDto = {
  id: number
  nombres: string
  apellidos: string
  documento: string
  fechaRegistro: string
  activo: boolean
}

const mapCliente = (cliente: ClienteDto): Cliente => ({
  id: cliente.id,
  nombres: cliente.nombres,
  apellidos: cliente.apellidos,
  documento: cliente.documento,
  nombre: cliente.nombres,
  apellido: cliente.apellidos,
  correo: "",
  telefono: cliente.documento,
  direccion: "",
  fechaRegistro: cliente.fechaRegistro,
  activo: cliente.activo,
})

const mapClientePayload = (data: ClienteCreate | ClienteUpdate) => ({
  nombres: data.nombre,
  apellidos: data.apellido,
  documento: data.documento || data.telefono || "",
})

export const clienteService = {
  /**
   * Obtener lista paginada de clientes
   */
  async getAll(params?: ClienteFilterParams): Promise<PaginatedResponse<Cliente>> {
    const response = await apiClient.get<ClienteDto[]>("/Cliente", { params })
    return {
      data: response.data.map(mapCliente),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener un cliente por ID
   */
  async getById(id: number): Promise<Cliente> {
    const response = await apiClient.get<ClienteDto>(`/Cliente/${id}`)
    return mapCliente(response.data)
  },

  /**
   * Crear un nuevo cliente
   */
  async create(data: ClienteCreate): Promise<Cliente> {
    const response = await apiClient.post<ClienteDto>("/Cliente", mapClientePayload(data))
    return mapCliente(response.data)
  },

  /**
   * Actualizar un cliente existente
   */
  async update(id: number, data: ClienteUpdate): Promise<Cliente> {
    const response = await apiClient.put<ClienteDto>(`/Cliente/${id}`, mapClientePayload(data))
    return mapCliente(response.data)
  },

  /**
   * Eliminar un cliente
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/Cliente/${id}`)
  },
}
