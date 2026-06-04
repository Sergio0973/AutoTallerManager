import apiClient from "../client"
import type { Auditoria, AuditoriaFilterParams, Usuario } from "../types"

const cleanParams = (params?: AuditoriaFilterParams) => {
  if (!params) return undefined

  return Object.fromEntries(
    Object.entries(params).filter(([, value]) => value !== undefined && value !== null && value !== "")
  )
}

export const auditoriaService = {
  async getAll(params?: AuditoriaFilterParams): Promise<Auditoria[]> {
    const response = await apiClient.get<Auditoria[]>("/Auditoria", {
      params: cleanParams(params),
    })
    return response.data
  },

  async getById(id: number): Promise<Auditoria> {
    const response = await apiClient.get<Auditoria>(`/Auditoria/${id}`)
    return response.data
  },

  async getUsuarios(): Promise<Usuario[]> {
    const response = await apiClient.get<Usuario[]>("/Usuario")
    return response.data
  },
}
