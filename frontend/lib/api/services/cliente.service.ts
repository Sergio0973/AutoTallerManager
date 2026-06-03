import apiClient, { getTotalCount } from "../client"
import { AxiosError } from "axios"
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

type ClienteCorreoDto = {
  id: number
  clienteId: number
  correo: string
  principal: boolean
}

type ClienteTelefonoDto = {
  id: number
  clienteId: number
  telefono: string
  tipo: string
}

type ClienteDireccionDto = {
  id: number
  clienteId: number
  ciudadId: number
  direccion: string
  principal: boolean
}

const mapCliente = (cliente: ClienteDto): Cliente => ({
  id: cliente.id,
  nombres: cliente.nombres,
  apellidos: cliente.apellidos,
  documento: cliente.documento,
  nombre: cliente.nombres,
  apellido: cliente.apellidos,
  correo: "",
  telefono: "",
  direccion: "",
  fechaRegistro: cliente.fechaRegistro,
  activo: cliente.activo,
})

const mapClientePayload = (data: ClienteCreate | ClienteUpdate) => ({
  nombres: data.nombre,
  apellidos: data.apellido,
  documento: data.documento || data.telefono || "",
})

const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (error instanceof AxiosError) {
    const data = error.response?.data

    if (data && typeof data === "object" && "message" in data && typeof data.message === "string") {
      return data.message
    }

    if (data && typeof data === "object" && "errors" in data) {
      return fallback
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

const pickPrincipal = <T extends { principal?: boolean }>(items: T[]) =>
  items.find((item) => item.principal) || items[0]

const withContactData = (
  cliente: Cliente,
  correos: ClienteCorreoDto[],
  telefonos: ClienteTelefonoDto[],
  direcciones: ClienteDireccionDto[]
): Cliente => {
  const correo = pickPrincipal(correos.filter((item) => item.clienteId === cliente.id))
  const telefono = telefonos.find((item) => item.clienteId === cliente.id)
  const direccion = pickPrincipal(direcciones.filter((item) => item.clienteId === cliente.id))

  return {
    ...cliente,
    correo: correo?.correo || "",
    correoId: correo?.id,
    telefono: telefono?.telefono || "",
    telefonoId: telefono?.id,
    direccion: direccion?.direccion || "",
    direccionId: direccion?.id,
    ciudadId: direccion?.ciudadId,
  }
}

const getContacts = async () => {
  const [correosResult, telefonosResult, direccionesResult] = await Promise.allSettled([
    apiClient.get<ClienteCorreoDto[]>("/ClienteCorreo"),
    apiClient.get<ClienteTelefonoDto[]>("/ClienteTelefono"),
    apiClient.get<ClienteDireccionDto[]>("/ClienteDireccion"),
  ])

  return {
    correos: correosResult.status === "fulfilled" ? correosResult.value.data : [],
    telefonos: telefonosResult.status === "fulfilled" ? telefonosResult.value.data : [],
    direcciones: direccionesResult.status === "fulfilled" ? direccionesResult.value.data : [],
  }
}

const saveContactData = async (clienteId: number, data: ClienteCreate | ClienteUpdate, existing?: Cliente) => {
  const operations: Array<{ label: string; operation: () => Promise<unknown> }> = []

  if (data.correo?.trim()) {
    const payload = { clienteId, correo: data.correo.trim(), principal: true }
    operations.push({
      label: "correo",
      operation: () =>
        existing?.correoId
          ? apiClient.put(`/ClienteCorreo/${existing.correoId}`, payload)
          : apiClient.post("/ClienteCorreo", payload),
    })
  }

  if (data.telefono?.trim()) {
    const payload = { clienteId, telefono: data.telefono.trim(), tipo: "Principal" }
    operations.push({
      label: "telefono",
      operation: () =>
        existing?.telefonoId
          ? apiClient.put(`/ClienteTelefono/${existing.telefonoId}`, payload)
          : apiClient.post("/ClienteTelefono", payload),
    })
  }

  if (data.direccion?.trim() && data.ciudadId && data.ciudadId > 0) {
    const payload = {
      clienteId,
      ciudadId: data.ciudadId,
      direccion: data.direccion.trim(),
      principal: true,
    }
    operations.push({
      label: "direccion",
      operation: () =>
        existing?.direccionId
          ? apiClient.put(`/ClienteDireccion/${existing.direccionId}`, payload)
          : apiClient.post("/ClienteDireccion", payload),
    })
  }

  for (const item of operations) {
    try {
      await item.operation()
    } catch (error) {
      throw new Error(getApiErrorMessage(error, `No se pudo guardar el ${item.label} del cliente.`))
    }
  }
}

export const clienteService = {
  /**
   * Obtener lista paginada de clientes
   */
  async getAll(params?: ClienteFilterParams): Promise<PaginatedResponse<Cliente>> {
    const [response, contacts] = await Promise.all([
      apiClient.get<ClienteDto[]>("/Cliente", { params }),
      getContacts(),
    ])
    const clientes = response.data
      .map(mapCliente)
      .map((cliente) => withContactData(cliente, contacts.correos, contacts.telefonos, contacts.direcciones))

    return {
      data: clientes,
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener un cliente por ID
   */
  async getById(id: number): Promise<Cliente> {
    const [response, correos, telefonos, direcciones] = await Promise.all([
      apiClient.get<ClienteDto>(`/Cliente/${id}`),
      apiClient.get<ClienteCorreoDto[]>("/ClienteCorreo", { params: { clienteId: id } }),
      apiClient.get<ClienteTelefonoDto[]>("/ClienteTelefono", { params: { clienteId: id } }),
      apiClient.get<ClienteDireccionDto[]>("/ClienteDireccion", { params: { clienteId: id } }),
    ])
    return withContactData(mapCliente(response.data), correos.data, telefonos.data, direcciones.data)
  },

  /**
   * Crear un nuevo cliente
   */
  async create(data: ClienteCreate): Promise<Cliente> {
    let response
    try {
      response = await apiClient.post<ClienteDto>("/Cliente", mapClientePayload(data))
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo crear el cliente."))
    }

    const cliente = mapCliente(response.data)
    await saveContactData(cliente.id, data)
    return this.getById(cliente.id)
  },

  /**
   * Actualizar un cliente existente
   */
  async update(id: number, data: ClienteUpdate): Promise<Cliente> {
    const existing = await this.getById(id)
    try {
      await apiClient.put<ClienteDto>(`/Cliente/${id}`, mapClientePayload(data))
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo actualizar el cliente."))
    }

    await saveContactData(id, data, existing)
    return this.getById(id)
  },

  /**
   * Eliminar un cliente
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/Cliente/${id}`)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo eliminar el cliente."))
    }
  },
}
