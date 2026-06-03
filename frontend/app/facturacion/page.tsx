"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { CreditCard, Receipt, Search, Wallet } from "lucide-react"
import { facturacionService } from "@/lib/api"
import type { EstadoFactura, Factura, MetodoPago, Pago } from "@/lib/api/types"

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 }).format(value)

const formatDate = (value?: string) => {
  if (!value) return "-"
  return new Date(value).toLocaleDateString("es-CO", {
    year: "numeric",
    month: "short",
    day: "numeric",
  })
}

export default function FacturacionPage() {
  const [facturas, setFacturas] = useState<Factura[]>([])
  const [pagos, setPagos] = useState<Pago[]>([])
  const [estados, setEstados] = useState<EstadoFactura[]>([])
  const [metodosPago, setMetodosPago] = useState<MetodoPago[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [facturasResponse, estadosResponse, metodosResponse] = await Promise.allSettled([
        facturacionService.getAll({ pageNumber: 1, pageSize: 100 }),
        facturacionService.getEstados(),
        facturacionService.getMetodosPago(),
      ])

      const facturasData = facturasResponse.status === "fulfilled" ? facturasResponse.value.data : []
      const estadosData = estadosResponse.status === "fulfilled" ? estadosResponse.value : []
      const metodosData = metodosResponse.status === "fulfilled" ? metodosResponse.value : []

      const pagosData = (
        await Promise.all(
          facturasData.map((factura) =>
            facturacionService.getPagosByFactura(factura.id).catch(() => [])
          )
        )
      ).flat()

      setFacturas(facturasData)
      setEstados(estadosData)
      setMetodosPago(metodosData)
      setPagos(pagosData)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const filteredFacturas = facturas.filter((factura) => {
    const text = searchTerm.toLowerCase()
    return (
      (factura.numero || "").toLowerCase().includes(text) ||
      factura.id.toString().includes(text) ||
      factura.ordenId.toString().includes(text) ||
      (factura.observaciones || "").toLowerCase().includes(text)
    )
  })

  const totalFacturado = facturas.reduce((total, factura) => total + factura.total, 0)
  const totalPagado = pagos.reduce((total, pago) => total + pago.monto, 0)

  return (
    <AppLayout>
      <Header title="Facturacion" subtitle="Consulta de facturas, totales y pagos registrados" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Receipt className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Facturas</p>
                <p className="text-xl font-bold">{facturas.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <Wallet className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total facturado</p>
                <p className="text-xl font-bold">{formatCurrency(totalFacturado)}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-chart-2/10">
                <CreditCard className="w-5 h-5 text-chart-2" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Pagos confirmados</p>
                <p className="text-xl font-bold">{formatCurrency(totalPagado)}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="relative w-full sm:w-80">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar por factura u orden..."
            value={searchTerm}
            onChange={(event) => setSearchTerm(event.target.value)}
            className="pl-9 bg-secondary border-border"
          />
        </div>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead>Factura</TableHead>
                  <TableHead>Orden</TableHead>
                  <TableHead>Fecha</TableHead>
                  <TableHead>Mano de obra</TableHead>
                  <TableHead>Repuestos</TableHead>
                  <TableHead>Total</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead>Pagos</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredFacturas.map((factura) => {
                  const pagosFactura = pagos.filter((pago) => pago.facturaId === factura.id)
                  const pagado = pagosFactura.reduce((total, pago) => total + pago.monto, 0)
                  const estado = estados.find((item) => item.id === (factura.estadoFacturaId ?? factura.estadoId))
                  return (
                    <TableRow key={factura.id} className="border-border">
                      <TableCell className="font-mono font-semibold">{factura.numero}</TableCell>
                      <TableCell>ORD-{factura.ordenId.toString().padStart(3, "0")}</TableCell>
                      <TableCell>{formatDate(factura.fechaEmision)}</TableCell>
                      <TableCell>{formatCurrency(factura.manoDeObra ?? 0)}</TableCell>
                      <TableCell>{formatCurrency(factura.costoRepuestos ?? 0)}</TableCell>
                      <TableCell className="font-semibold">{formatCurrency(factura.total)}</TableCell>
                      <TableCell>
                        <Badge className="bg-primary/20 text-primary">{estado?.nombre || factura.estadoFacturaId}</Badge>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{formatCurrency(pagado)}</div>
                          <div className="text-muted-foreground">
                            {pagosFactura.length
                              ? pagosFactura.map((pago) => metodosPago.find((metodo) => metodo.id === pago.metodoPagoId)?.nombre || pago.metodoPagoId).join(", ")
                              : "Sin pagos"}
                          </div>
                        </div>
                      </TableCell>
                    </TableRow>
                  )
                })}
                {!isLoading && filteredFacturas.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={8} className="text-center py-8 text-muted-foreground">
                      No se encontraron facturas
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>
    </AppLayout>
  )
}
