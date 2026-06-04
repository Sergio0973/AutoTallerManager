import apiClient from "../client"
import type { LoginRequest, LoginResponse } from "../types"
import Cookies from "js-cookie"

export const authService = {
  /**
   * Iniciar sesión con correo y contraseña
   */
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>("/Auth/login", credentials)
    
    // Guardar token y datos del usuario en cookies
    Cookies.set("auth_token", response.data.token, { 
      expires: 7, // 7 días
      sameSite: "strict",
      path: "/",
    })
    Cookies.set("user", JSON.stringify(response.data.usuario), {
      expires: 7,
      sameSite: "strict",
      path: "/",
    })
    
    return response.data
  },

  /**
   * Cerrar sesión
   */
  logout(): void {
    Cookies.remove("auth_token", { path: "/" })
    Cookies.remove("user", { path: "/" })
    if (typeof window !== "undefined") {
      window.location.href = "/login"
    }
  },

  /**
   * Verificar si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    return !!Cookies.get("auth_token")
  },

  /**
   * Obtener token actual
   */
  getToken(): string | undefined {
    return Cookies.get("auth_token")
  },

  /**
   * Obtener datos del usuario actual desde cookies
   */
  getCurrentUser() {
    if (typeof window === "undefined") {
      return null
    }

    try {
      const userStr = Cookies.get("user")
      if (!userStr) {
        return null
      }

      return JSON.parse(userStr)
    } catch {
      Cookies.remove("auth_token", { path: "/" })
      Cookies.remove("user", { path: "/" })
      return null
    }
  },

  /**
   * Verificar si el usuario tiene un rol específico
   */
  hasRole(roleName: string): boolean {
    const user = this.getCurrentUser()
    const currentRole = typeof user?.rol === "string" ? user.rol : user?.rol?.nombre
    return currentRole?.toLowerCase() === roleName.toLowerCase()
  },

  /**
   * Verificar si es administrador
   */
  isAdmin(): boolean {
    return this.hasRole("Admin")
  },

  /**
   * Verificar si es recepcionista
   */
  isRecepcionista(): boolean {
    return this.hasRole("Recepcionista")
  },

  /**
   * Verificar si es mecánico
   */
  isMecanico(): boolean {
    return this.hasRole("Mecanico")
  },
}
