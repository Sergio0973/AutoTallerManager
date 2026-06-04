"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Alert, AlertDescription } from "@/components/ui/alert"
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { CreditCard, Loader2, Plus, Receipt, Search, Wallet } from "lucide-react"
import { facturacionService, ordenService } from "@/lib/api"
import { useAuth } from "@/contexts/auth-context"
import type { EstadoFactura, Factura, MetodoPago, OrdenServicio, Pago } from "@/lib/api/types"

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
  const { user } = useAuth()
  const [facturas, setFacturas] = useState<Factura[]>([])
  const [pagos, setPagos] = useState<Pago[]>([])
  const [estados, setEstados] = useState<EstadoFactura[]>([])
  const [metodosPago, setMetodosPago] = useState<MetodoPago[]>([])
  const [ordenes, setOrdenes] = useState<OrdenServicio[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [isFacturaOpen, setIsFacturaOpen] = useState(false)
  const [isPagoOpen, setIsPagoOpen] = useState(false)
  const [selectedFactura, setSelectedFactura] = useState<Factura | null>(null)
  const [errorMessage, setErrorMessage] = useState("")
  const [facturaForm, setFacturaForm] = useState({
    ordenId: 0,
    estadoFacturaId: 0,
    descuento: 0,
    impuestoPct: 19,
    observaciones: "",
  })
  const [pagoForm, setPagoForm] = useState({
    metodoPagoId: 0,
    monto: 0,
    referencia: "",
    estado: "Confirmado",
  })

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [facturasResponse, estadosResponse, metodosResponse, ordenesResponse] = await Promise.allSettled([
        facturacionService.getAll({ pageNumber: 1, pageSize: 100 }),
        facturacionService.getEstados(),
        facturacionService.getMetodosPago(),
        ordenService.getAll({ pageNumber: 1, pageSize: 100 }),
      ])

      const facturasData: Factura[] = facturasResponse.status === "fulfilled" ? facturasResponse.value.data : []
      const estadosData: EstadoFactura[] = estadosResponse.status === "fulfilled" ? estadosResponse.value : []
      const metodosData: MetodoPago[] = metodosResponse.status === "fulfilled" ? metodosResponse.value : []
      const ordenesData: OrdenServicio[] = ordenesResponse.status === "fulfilled" ? ordenesResponse.value.data : []

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
      setOrdenes(ordenesData)
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

  const openFacturaDialog = () => {
    const estadoEmitida = estados.find((estado) => estado.nombre.toLowerCase().includes("emit"))
    setErrorMessage("")
    setFacturaForm({
      ordenId: ordenes[0]?.id || 0,
      estadoFacturaId: estadoEmitida?.id || estados[0]?.id || 0,
      descuento: 0,
      impuestoPct: 19,
      observaciones: "",
    })
    setIsFacturaOpen(true)
  }

  const openPagoDialog = (factura: Factura) => {
    const metodoEfectivo = metodosPago.find((metodo) => metodo.nombre.toLowerCase().includes("efect"))
    const pagado = pagos
      .filter((pago) => pago.facturaId === factura.id)
      .reduce((total, pago) => total + pago.monto, 0)

    setSelectedFactura(factura)
    setErrorMessage("")
    setPagoForm({
      metodoPagoId: metodoEfectivo?.id || metodosPago[0]?.id || 0,
      monto: Math.max(factura.total - pagado, 0),
      referencia: `PAGO-FAC-${factura.id.toString().padStart(3, "0")}`,
      estado: "Confirmado",
    })
    setIsPagoOpen(true)
  }

  const handleCreateFactura = async () => {
    setIsLoading(true)
    setErrorMessage("")
    try {
      await facturacionService.create({
        ordenId: facturaForm.ordenId,
        estadoFacturaId: facturaForm.estadoFacturaId,
        usuarioId: user?.id,
        descuento: facturaForm.descuento,
        impuestoPct: facturaForm.impuestoPct,
        fechaEmision: new Date().toISOString().split("T")[0],
        observaciones: facturaForm.observaciones,
      })
      setIsFacturaOpen(false)
      await loadData()
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo crear la factura.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleCreatePago = async () => {
    if (!selectedFactura) return
    setIsLoading(true)
    setErrorMessage("")
    try {
      await facturacionService.registrarPago({
        facturaId: selectedFactura.id,
        metodoPagoId: pagoForm.metodoPagoId,
        monto: pagoForm.monto,
        referencia: pagoForm.referencia,
        estado: pagoForm.estado,
      })
      setIsPagoOpen(false)
      await loadData()
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo registrar el pago.")
    } finally {
      setIsLoading(false)
    }
  }

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

        <div className="flex flex-col sm:flex-row gap-3 sm:items-center sm:justify-between">
          <div className="relative w-full sm:w-80">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por factura u orden..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="pl-9 bg-secondary border-border"
            />
          </div>
          <Button onClick={openFacturaDialog} className="bg-primary text-primary-foreground">
            <Plus className="w-4 h-4 mr-2" />
            Nueva Factura
          </Button>
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
                  <TableHead className="text-right">Acciones</TableHead>
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
                      <TableCell className="text-right">
                        <Button variant="outline" size="sm" onClick={() => openPagoDialog(factura)}>
                          Registrar pago
                        </Button>
                      </TableCell>
                    </TableRow>
                  )
                })}
                {!isLoading && filteredFacturas.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={9} className="text-center py-8 text-muted-foreground">
                      No se encontraron facturas
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>

      <Dialog open={isFacturaOpen} onOpenChange={setIsFacturaOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nueva Factura</DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Orden</label>
              <Select
                value={facturaForm.ordenId.toString()}
                onValueChange={(value) => setFacturaForm({ ...facturaForm, ordenId: parseInt(value) })}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar orden" />
                </SelectTrigger>
                <SelectContent>
                  {ordenes.map((orden) => (
                    <SelectItem key={orden.id} value={orden.id.toString()}>
                      ORD-{orden.id.toString().padStart(3, "0")} - {orden.descripcionProblema || "Sin observaciones"}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Estado</label>
                <Select
                  value={facturaForm.estadoFacturaId.toString()}
                  onValueChange={(value) =>
                    setFacturaForm({ ...facturaForm, estadoFacturaId: parseInt(value) })
                  }
                >
                  <SelectTrigger className="bg-secondary border-border">
                    <SelectValue placeholder="Estado" />
                  </SelectTrigger>
                  <SelectContent>
                    {estados.map((estado) => (
                      <SelectItem key={estado.id} value={estado.id.toString()}>
                        {estado.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Impuesto %</label>
                <Input
                  type="number"
                  value={facturaForm.impuestoPct}
                  onChange={(event) =>
                    setFacturaForm({ ...facturaForm, impuestoPct: Number(event.target.value) || 0 })
                  }
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Descuento</label>
              <Input
                inputMode="numeric"
                value={facturaForm.descuento}
                onChange={(event) =>
                  setFacturaForm({
                    ...facturaForm,
                    descuento: Number(event.target.value.replace(/\D/g, "")) || 0,
                  })
                }
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Observaciones</label>
              <Textarea
                value={facturaForm.observaciones}
                onChange={(event) => setFacturaForm({ ...facturaForm, observaciones: event.target.value })}
                placeholder="Detalle de la factura..."
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsFacturaOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreateFactura}
              disabled={isLoading || !facturaForm.ordenId || !facturaForm.estadoFacturaId}
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Crear factura
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isPagoOpen} onOpenChange={setIsPagoOpen}>
        <DialogContent className="bg-card border-border max-w-md">
          <DialogHeader>
            <DialogTitle>Registrar Pago</DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Metodo de pago</label>
              <Select
                value={pagoForm.metodoPagoId.toString()}
                onValueChange={(value) => setPagoForm({ ...pagoForm, metodoPagoId: parseInt(value) })}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar metodo" />
                </SelectTrigger>
                <SelectContent>
                  {metodosPago.map((metodo) => (
                    <SelectItem key={metodo.id} value={metodo.id.toString()}>
                      {metodo.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Monto</label>
              <Input
                inputMode="numeric"
                value={pagoForm.monto}
                onChange={(event) =>
                  setPagoForm({ ...pagoForm, monto: Number(event.target.value.replace(/\D/g, "")) || 0 })
                }
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Referencia</label>
              <Input
                value={pagoForm.referencia}
                onChange={(event) => setPagoForm({ ...pagoForm, referencia: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsPagoOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreatePago}
              disabled={isLoading || !pagoForm.metodoPagoId || pagoForm.monto <= 0}
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Registrar pago
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
