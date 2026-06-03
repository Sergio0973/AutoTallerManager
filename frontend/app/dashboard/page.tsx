"use client"

import { useEffect, useMemo, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import {
  AlertTriangle,
  Car,
  CheckCircle,
  ClipboardList,
  Clock,
  DollarSign,
  Package,
  Users,
} from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { clienteService, citaService, facturacionService, inventarioService, ordenService, vehiculoService } from "@/lib/api"
import type { Cita, EstadoOrden, Factura, OrdenServicio, Repuesto, Vehiculo } from "@/lib/api/types"

type DashboardData = {
  clientesTotal: number
  vehiculos: Vehiculo[]
  ordenes: OrdenServicio[]
  estadosOrden: EstadoOrden[]
  repuestosBajoStock: Repuesto[]
  facturas: Factura[]
  citas: Cita[]
}

const emptyDashboard: DashboardData = {
  clientesTotal: 0,
  vehiculos: [],
  ordenes: [],
  estadosOrden: [],
  repuestosBajoStock: [],
  facturas: [],
  citas: [],
}

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("es-CO", {
    style: "currency",
    currency: "COP",
    maximumFractionDigits: 0,
  }).format(value)

const isToday = (value?: string) => {
  if (!value) return false
  return value.split("T")[0] === new Date().toISOString().split("T")[0]
}

const estadoColor = (estado?: string) => {
  const normalized = estado?.toLowerCase() || ""
  if (normalized.includes("cancel")) return "bg-destructive/20 text-destructive"
  if (normalized.includes("lista") || normalized.includes("complet") || normalized.includes("entreg")) {
    return "bg-success/20 text-success"
  }
  if (normalized.includes("diagn") || normalized.includes("repar") || normalized.includes("proceso")) {
    return "bg-primary/20 text-primary"
  }
  return "bg-warning/20 text-warning"
}

export default function DashboardPage() {
  const [data, setData] = useState<DashboardData>(emptyDashboard)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const loadDashboard = async () => {
      setIsLoading(true)
      const [clientesResult, vehiculosResult, ordenesResult, estadosOrdenResult, repuestosResult, facturasResult, citasResult] =
        await Promise.allSettled([
          clienteService.getAll({ pageNumber: 1, pageSize: 1 }),
          vehiculoService.getAll({ pageNumber: 1, pageSize: 100 }),
          ordenService.getAll({ pageNumber: 1, pageSize: 100 }),
          ordenService.getEstados(),
          inventarioService.getLowStock(),
          facturacionService.getAll({ pageNumber: 1, pageSize: 100 }),
          citaService.getAll({ pageNumber: 1, pageSize: 100 }),
        ])

      const estadosOrden: EstadoOrden[] = estadosOrdenResult.status === "fulfilled" ? estadosOrdenResult.value : []
      const ordenesBase: OrdenServicio[] = ordenesResult.status === "fulfilled" ? ordenesResult.value.data : []
      const ordenes: OrdenServicio[] = ordenesBase.map((orden: OrdenServicio) => ({
            ...orden,
            estado: estadosOrden.find((estado: EstadoOrden) => estado.id === orden.estadoId),
          }))

      setData({
        clientesTotal: clientesResult.status === "fulfilled" ? clientesResult.value.totalCount || clientesResult.value.data.length : 0,
        vehiculos: vehiculosResult.status === "fulfilled" ? vehiculosResult.value.data : [],
        ordenes,
        estadosOrden,
        repuestosBajoStock: repuestosResult.status === "fulfilled" ? repuestosResult.value : [],
        facturas: facturasResult.status === "fulfilled" ? facturasResult.value.data : [],
        citas: citasResult.status === "fulfilled" ? citasResult.value.data : [],
      })
      setIsLoading(false)
    }

    loadDashboard()
  }, [])

  const stats = useMemo(() => {
    const ordenesActivas = data.ordenes.filter((orden) => {
      const estado = orden.estado?.nombre?.toLowerCase() || ""
      return !estado.includes("cancel") && !estado.includes("entreg") && !estado.includes("complet")
    }).length

    return [
      {
        title: "Clientes Activos",
        value: data.clientesTotal.toString(),
        detail: "Registrados en backend",
        trend: "up",
        icon: Users,
      },
      {
        title: "Vehiculos Registrados",
        value: data.vehiculos.length.toString(),
        detail: "Asociados a clientes",
        trend: "up",
        icon: Car,
      },
      {
        title: "Ordenes Activas",
        value: ordenesActivas.toString(),
        detail: "No cerradas ni canceladas",
        trend: "warning",
        icon: ClipboardList,
      },
      {
        title: "Repuestos Bajo Stock",
        value: data.repuestosBajoStock.length.toString(),
        detail: "Segun inventario",
        trend: "warning",
        icon: Package,
      },
    ]
  }, [data])

  const ordenesRecientes = data.ordenes.slice(0, 5)
  const completadasHoy = data.ordenes.filter((orden) => isToday(orden.fechaEntregaReal || orden.fechaEntrega)).length
  const citasHoy = data.citas.filter((cita) => isToday(cita.fechaCita || cita.fechaHora)).length
  const facturadoHoy = data.facturas
    .filter((factura) => isToday(factura.fechaEmision))
    .reduce((total, factura) => total + factura.total, 0)

  return (
    <AppLayout>
      <Header title="Dashboard" subtitle="Vista general del taller con datos del backend" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {stats.map((stat) => (
            <Card key={stat.title} className="bg-card border-border">
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-muted-foreground">{stat.title}</p>
                    <p className="text-2xl font-bold text-foreground mt-1">
                      {isLoading ? "..." : stat.value}
                    </p>
                    <p className={stat.trend === "warning" ? "text-xs mt-1 text-warning" : "text-xs mt-1 text-success"}>
                      {stat.detail}
                    </p>
                  </div>
                  <div className={stat.trend === "warning" ? "flex items-center justify-center w-12 h-12 rounded-lg bg-warning/10" : "flex items-center justify-center w-12 h-12 rounded-lg bg-primary/10"}>
                    <stat.icon className={stat.trend === "warning" ? "w-6 h-6 text-warning" : "w-6 h-6 text-primary"} />
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <Card className="lg:col-span-2 bg-card border-border">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold flex items-center gap-2">
                <ClipboardList className="w-5 h-5 text-primary" />
                Ordenes Recientes
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {ordenesRecientes.map((orden) => (
                  <div
                    key={orden.id}
                    className="flex items-center justify-between p-3 rounded-lg bg-secondary/50 hover:bg-secondary transition-colors"
                  >
                    <div className="flex items-center gap-4">
                      <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                        <ClipboardList className="w-5 h-5 text-primary" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">
                          ORD-{orden.id.toString().padStart(3, "0")} - Vehiculo #{orden.vehiculoId}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {orden.observaciones || orden.descripcionProblema || "Sin observaciones"}
                        </p>
                      </div>
                    </div>
                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${estadoColor(orden.estado?.nombre)}`}>
                      {orden.estado?.nombre || `Estado ${orden.estadoId}`}
                    </span>
                  </div>
                ))}
                {!isLoading && ordenesRecientes.length === 0 && (
                  <div className="p-6 text-center text-sm text-muted-foreground">
                    No hay ordenes disponibles para este rol.
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          <Card className="bg-card border-border">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold flex items-center gap-2">
                <AlertTriangle className="w-5 h-5 text-warning" />
                Stock Bajo
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {data.repuestosBajoStock.slice(0, 4).map((item) => (
                  <div
                    key={item.id}
                    className="p-3 rounded-lg bg-warning/5 border border-warning/20"
                  >
                    <p className="text-sm font-medium text-foreground">
                      {item.descripcion || item.nombre}
                    </p>
                    <div className="flex items-center justify-between mt-2">
                      <span className="text-xs text-muted-foreground">
                        Stock: {item.stockActual} / Min: {item.stockMinimo}
                      </span>
                      <span className="text-xs text-warning font-medium">Reabastecer</span>
                    </div>
                    <div className="mt-2 h-1.5 bg-secondary rounded-full overflow-hidden">
                      <div
                        className="h-full bg-warning rounded-full"
                        style={{ width: `${Math.min(100, (item.stockActual / Math.max(item.stockMinimo, 1)) * 100)}%` }}
                      />
                    </div>
                  </div>
                ))}
                {!isLoading && data.repuestosBajoStock.length === 0 && (
                  <div className="p-6 text-center text-sm text-muted-foreground">
                    Sin repuestos bajo stock o sin acceso para este rol.
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-success/10">
                  <CheckCircle className="w-6 h-6 text-success" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Completadas Hoy</p>
                  <p className="text-2xl font-bold text-foreground">{isLoading ? "..." : completadasHoy}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-primary/10">
                  <Clock className="w-6 h-6 text-primary" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Citas Hoy</p>
                  <p className="text-2xl font-bold text-foreground">{isLoading ? "..." : citasHoy}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-success/10">
                  <DollarSign className="w-6 h-6 text-success" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Facturado Hoy</p>
                  <p className="text-2xl font-bold text-foreground">
                    {isLoading ? "..." : formatCurrency(facturadoHoy)}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </AppLayout>
  )
}
