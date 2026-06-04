import apiClient, { getTotalCount } from "../client"
import { AxiosError } from "axios"
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

const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (error instanceof AxiosError) {
    const data = error.response?.data

    if (data && typeof data === "object" && "message" in data && typeof data.message === "string") {
      if ("errors" in data && Array.isArray(data.errors) && data.errors.length > 0) {
        const details = data.errors
          .map((item: unknown) => {
            if (item && typeof item === "object" && "errorMessage" in item) {
              return String(item.errorMessage)
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
    try {
      const response = await apiClient.post<CitaDto>("/Cita", mapCitaPayload(data))
      return mapCita(response.data)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo crear la cita."))
    }
  },

  /**
   * Actualizar una cita
   */
  async update(id: number, data: Partial<CitaCreate>): Promise<Cita> {
    try {
      await apiClient.put(`/Cita/${id}`, mapCitaPayload(data))
      return this.getById(id)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo actualizar la cita."))
    }
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
    try {
      await apiClient.delete(`/Cita/${id}`)
    } catch (error) {
      throw new Error(getApiErrorMessage(error, "No se pudo eliminar la cita."))
    }
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
