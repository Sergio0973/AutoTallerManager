import { AxiosError } from "axios"
import apiClient from "../client"
import type {
  Rol,
  RolCreate,
  RolUpdate,
  Usuario,
  UsuarioCreate,
  UsuarioPasswordReset,
  UsuarioUpdate,
} from "../types"

export const getApiErrorMessage = (error: unknown, fallback: string) => {
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

export const usuarioService = {
  async getAll(): Promise<Usuario[]> {
    const response = await apiClient.get<Usuario[]>("/Usuario")
    return response.data
  },

  async create(data: UsuarioCreate): Promise<Usuario> {
    const response = await apiClient.post<Usuario>("/Usuario", data)
    return response.data
  },

  async update(id: number, data: UsuarioUpdate): Promise<void> {
    await apiClient.put(`/Usuario/${id}`, data)
  },

  async resetPassword(id: number, data: UsuarioPasswordReset): Promise<void> {
    await apiClient.put(`/Usuario/${id}/password`, data)
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/Usuario/${id}`)
  },

  async getRoles(): Promise<Rol[]> {
    const response = await apiClient.get<Rol[]>("/Rol")
    return response.data
  },

  async createRol(data: RolCreate): Promise<Rol> {
    const response = await apiClient.post<Rol>("/Rol", data)
    return response.data
  },

  async updateRol(id: number, data: RolUpdate): Promise<void> {
    await apiClient.put(`/Rol/${id}`, data)
  },
}
