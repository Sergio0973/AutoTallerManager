import axios, { AxiosError, InternalAxiosRequestConfig } from "axios"
import Cookies from "js-cookie"

// Configuración base de la API
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "/backend-api"

// Crear instancia de Axios
export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000,
})

// Interceptor para agregar token JWT a cada solicitud
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = Cookies.get("auth_token")
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Interceptor para manejar respuestas y errores
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Token expirado o inválido
      Cookies.remove("auth_token")
      Cookies.remove("user")
      if (typeof window !== "undefined") {
        window.location.href = "/login"
      }
    }
    return Promise.reject(error)
  }
)

// Tipos base para respuestas paginadas
export interface PaginatedResponse<T> {
  data: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
}

// Helper para obtener total count del header
export function getTotalCount(headers: Record<string, string>): number {
  return parseInt(headers["x-total-count"] || "0", 10)
}

export default apiClient
