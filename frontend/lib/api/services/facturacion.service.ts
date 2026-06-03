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

type FacturaDto = {
  id: number
  ordenId: number
  estadoFacturaId: number
  usuarioId: number
  manoDeObra: number
  costoRepuestos: number
  descuento: number
  impuestoPct: number
  subtotal: number
  total: number
  fechaEmision: string
  observaciones?: string | null
}

type PagoDto = {
  id: number
  facturaId: number
  metodoPagoId: number
  monto: number
  fechaPago: string
  referencia?: string | null
  estado: string
}

const mapFactura = (factura: FacturaDto): Factura => ({
  id: factura.id,
  numero: `FAC-${factura.id.toString().padStart(3, "0")}`,
  ordenId: factura.ordenId,
  estadoFacturaId: factura.estadoFacturaId,
  estadoId: factura.estadoFacturaId,
  usuarioId: factura.usuarioId,
  manoDeObra: factura.manoDeObra,
  costoRepuestos: factura.costoRepuestos,
  descuento: factura.descuento,
  impuestoPct: factura.impuestoPct,
  fechaEmision: factura.fechaEmision,
  subtotal: factura.subtotal,
  impuesto: factura.total - factura.subtotal,
  total: factura.total,
  observaciones: factura.observaciones ?? "",
})

const mapPago = (pago: PagoDto): Pago => ({
  id: pago.id,
  facturaId: pago.facturaId,
  metodoPagoId: pago.metodoPagoId,
  monto: pago.monto,
  fechaPago: pago.fechaPago,
  referencia: pago.referencia ?? "",
  estado: pago.estado,
  confirmado: pago.estado.toLowerCase() === "confirmado",
})

const mapFacturaPayload = (data: FacturaCreate) => ({
  ordenId: data.ordenId,
  estadoFacturaId: data.estadoFacturaId,
  usuarioId: data.usuarioId,
  descuento: data.descuento ?? 0,
  impuestoPct: data.impuestoPct ?? 19,
  fechaEmision: data.fechaEmision ?? new Date().toISOString().split("T")[0],
  observaciones: data.observaciones,
})

const mapPagoPayload = (data: PagoCreate) => ({
  facturaId: data.facturaId,
  metodoPagoId: data.metodoPagoId,
  monto: data.monto,
  referencia: data.referencia,
  estado: data.estado ?? "Confirmado",
})

export const facturacionService = {
  /**
   * Obtener lista paginada de facturas
   */
  async getAll(params?: PaginationParams): Promise<PaginatedResponse<Factura>> {
    const response = await apiClient.get<FacturaDto[]>("/Factura", { params })
    return {
      data: response.data.map(mapFactura),
      totalCount: getTotalCount(response.headers as Record<string, string>),
      pageNumber: params?.pageNumber || 1,
      pageSize: params?.pageSize || 20,
    }
  },

  /**
   * Obtener una factura por ID
   */
  async getById(id: number): Promise<Factura> {
    const response = await apiClient.get<FacturaDto>(`/Factura/${id}`)
    return mapFactura(response.data)
  },

  /**
   * Crear una nueva factura desde una orden
   */
  async create(data: FacturaCreate): Promise<Factura> {
    const response = await apiClient.post<FacturaDto>("/Factura", mapFacturaPayload(data))
    return mapFactura(response.data)
  },

  /**
   * Anular una factura
   */
  async anular(id: number): Promise<Factura> {
    const response = await apiClient.put<FacturaDto>(`/Factura/${id}`, { estadoFacturaId: id })
    return mapFactura(response.data)
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
    const response = await apiClient.post<PagoDto>("/Pago", mapPagoPayload(data))
    return mapPago(response.data)
  },

  /**
   * Confirmar un pago
   */
  async confirmarPago(id: number): Promise<Pago> {
    await apiClient.put(`/Pago/${id}`, { estado: "Confirmado" })
    const response = await apiClient.get<PagoDto>(`/Pago/${id}`)
    return mapPago(response.data)
  },

  /**
   * Obtener pagos de una factura
   */
  async getPagosByFactura(facturaId: number): Promise<Pago[]> {
    const response = await apiClient.get<PagoDto[]>("/Pago", { params: { facturaId } })
    return response.data.map(mapPago)
  },
}
