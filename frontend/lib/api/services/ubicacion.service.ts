import apiClient from "../client"
import type { Ciudad, CiudadCreate, Departamento, DepartamentoCreate, Pais, PaisCreate } from "../types"

export const ubicacionService = {
  async getPaises(): Promise<Pais[]> {
    const response = await apiClient.get<Pais[]>("/Pais")
    return response.data
  },

  async getDepartamentos(paisId?: number): Promise<Departamento[]> {
    const response = await apiClient.get<Departamento[]>("/Departamento", {
      params: paisId ? { paisId } : undefined,
    })
    return response.data
  },

  async getCiudades(departamentoId?: number): Promise<Ciudad[]> {
    const response = await apiClient.get<Ciudad[]>("/Ciudad", {
      params: departamentoId ? { departamentoId } : undefined,
    })
    return response.data
  },

  async createPais(data: PaisCreate): Promise<Pais> {
    const response = await apiClient.post<Pais>("/Pais", data)
    return response.data
  },

  async createDepartamento(data: DepartamentoCreate): Promise<Departamento> {
    const response = await apiClient.post<Departamento>("/Departamento", data)
    return response.data
  },

  async createCiudad(data: CiudadCreate): Promise<Ciudad> {
    const response = await apiClient.post<Ciudad>("/Ciudad", data)
    return response.data
  },
}
