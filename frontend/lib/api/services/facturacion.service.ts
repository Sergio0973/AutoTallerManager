import apiClient, { getTotalCount } from "../client"
import type { 
  Factura, 
  FacturaCreate, 
  Pago,
  PagoCreate,
  PaginatedResponse,
  PaginationParams,
  EstadoFactura,
  MetodoPago
} from "../types"

export const facturacionService = {
  /**
   * Obtener lista paginada de facturas
   */
  async getAll(params?: PaginationParams): Promise<PaginatedResponse<Factura>> {
    const response = await apiClient.get<Factura[]>("/Factura", { params })
    return {
      data: response.data,
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener una factura por ID
   */
  async getById(id: number): Promise<Factura> {
    const response = await apiClient.get<Factura>(`/Factura/${id}`)
    return response.data
  },

  /**
   * Crear una nueva factura desde una orden
   */
  async create(data: FacturaCreate): Promise<Factura> {
    const response = await apiClient.post<Factura>("/Factura", data)
    return response.data
  },

  /**
   * Anular una factura
   */
  async anular(id: number): Promise<Factura> {
    const response = await apiClient.patch<Factura>(`/Factura/${id}/anular`)
    return response.data
  },

  /**
   * Obtener estados de factura
   */
  async getEstados(): Promise<EstadoFactura[]> {
    const response = await apiClient.get<EstadoFactura[]>("/EstadoFactura")
    return response.data
  },

  /**
   * Obtener métodos de pago
   */
  async getMetodosPago(): Promise<MetodoPago[]> {
    const response = await apiClient.get<MetodoPago[]>("/MetodoPago")
    return response.data
  },

  // Pagos
  /**
   * Registrar un pago
   */
  async registrarPago(data: PagoCreate): Promise<Pago> {
    const response = await apiClient.post<Pago>("/Pago", data)
    return response.data
  },

  /**
   * Confirmar un pago
   */
  async confirmarPago(id: number): Promise<Pago> {
    const response = await apiClient.patch<Pago>(`/Pago/${id}/confirmar`)
    return response.data
  },

  /**
   * Obtener pagos de una factura
   */
  async getPagosByFactura(facturaId: number): Promise<Pago[]> {
    const response = await apiClient.get<Pago[]>(`/Factura/${facturaId}/pagos`)
    return response.data
  },
}
