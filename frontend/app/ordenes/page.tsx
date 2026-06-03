"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Alert, AlertDescription } from "@/components/ui/alert"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { 
  Search, 
  Plus, 
  MoreHorizontal, 
  Pencil, 
  Eye,
  ClipboardList,
  Car,
  User,
  Wrench,
  Clock,
  CheckCircle,
  XCircle,
  AlertCircle,
  ChevronLeft,
  ChevronRight,
  Loader2,
  Calendar,
  FileText,
  Receipt
} from "lucide-react"
import { clienteService, ordenService, vehiculoService } from "@/lib/api"
import { useAuth } from "@/contexts/auth-context"
import type { OrdenServicio, EstadoOrden, Vehiculo, Cliente } from "@/lib/api/types"

// Mock data
const mockEstados: EstadoOrden[] = [
  { id: 1, nombre: "Pendiente" },
  { id: 2, nombre: "En proceso" },
  { id: 3, nombre: "Completada" },
  { id: 4, nombre: "Cancelada" },
]

const mockClientes: Cliente[] = [
  { id: 1, nombre: "Carlos", apellido: "Mendoza", correo: "carlos@email.com", telefono: "+1 555-0101", fechaRegistro: "2024-01-15", activo: true },
  { id: 2, nombre: "María", apellido: "García", correo: "maria@email.com", telefono: "+1 555-0102", fechaRegistro: "2024-02-20", activo: true },
  { id: 3, nombre: "Juan", apellido: "Pérez", correo: "juan@email.com", telefono: "+1 555-0103", fechaRegistro: "2024-03-10", activo: true },
]

const mockVehiculos: Vehiculo[] = [
  { id: 1, placa: "ABC-123", anio: 2020, kilometraje: 45000, clienteId: 1, marcaId: 1, modeloId: 1, fechaRegistro: "2024-01-20", marca: { id: 1, nombre: "Toyota" }, modelo: { id: 1, nombre: "Corolla", marcaId: 1 }, cliente: mockClientes[0] },
  { id: 2, placa: "XYZ-789", anio: 2019, kilometraje: 62000, clienteId: 2, marcaId: 2, modeloId: 4, fechaRegistro: "2024-02-25", marca: { id: 2, nombre: "Honda" }, modelo: { id: 4, nombre: "Civic", marcaId: 2 }, cliente: mockClientes[1] },
  { id: 3, placa: "DEF-456", anio: 2021, kilometraje: 28000, clienteId: 3, marcaId: 3, modeloId: 7, fechaRegistro: "2024-03-15", marca: { id: 3, nombre: "Ford" }, modelo: { id: 7, nombre: "Focus", marcaId: 3 }, cliente: mockClientes[2] },
]

const mockOrdenes: OrdenServicio[] = [
  {
    id: 1,
    fechaIngreso: "2024-06-01T08:00:00Z",
    fechaEstimadaEntrega: "2024-06-03T18:00:00Z",
    descripcionProblema: "El motor hace un ruido extraño al acelerar",
    diagnostico: "Se detectó desgaste en la correa de distribución",
    kilometrajeIngreso: 45200,
    vehiculoId: 1,
    vehiculo: mockVehiculos[0],
    estadoId: 2,
    estado: mockEstados[1],
  },
  {
    id: 2,
    fechaIngreso: "2024-06-02T10:30:00Z",
    fechaEstimadaEntrega: "2024-06-02T16:00:00Z",
    descripcionProblema: "Cambio de aceite y revisión general",
    kilometrajeIngreso: 62500,
    vehiculoId: 2,
    vehiculo: mockVehiculos[1],
    estadoId: 1,
    estado: mockEstados[0],
  },
  {
    id: 3,
    fechaIngreso: "2024-05-28T09:00:00Z",
    fechaEstimadaEntrega: "2024-05-30T17:00:00Z",
    fechaEntrega: "2024-05-30T15:30:00Z",
    descripcionProblema: "Frenos hacen ruido al frenar",
    diagnostico: "Pastillas de freno desgastadas",
    kilometrajeIngreso: 28500,
    vehiculoId: 3,
    vehiculo: mockVehiculos[2],
    estadoId: 3,
    estado: mockEstados[2],
  },
  {
    id: 4,
    fechaIngreso: "2024-05-25T14:00:00Z",
    descripcionProblema: "Revisión de suspensión",
    kilometrajeIngreso: 45100,
    vehiculoId: 1,
    vehiculo: mockVehiculos[0],
    estadoId: 4,
    estado: mockEstados[3],
  },
]

const estadoConfig: Record<string, { color: string; icon: typeof Clock }> = {
  Pendiente: { color: "bg-warning/20 text-warning", icon: Clock },
  "En proceso": { color: "bg-primary/20 text-primary", icon: Wrench },
  Completada: { color: "bg-success/20 text-success", icon: CheckCircle },
  Cancelada: { color: "bg-destructive/20 text-destructive", icon: XCircle },
  RECIBIDA: { color: "bg-warning/20 text-warning", icon: Clock },
  DIAGNOSTICO: { color: "bg-primary/20 text-primary", icon: AlertCircle },
  REPARACION: { color: "bg-primary/20 text-primary", icon: Wrench },
  LISTA: { color: "bg-success/20 text-success", icon: CheckCircle },
  ENTREGADA: { color: "bg-success/20 text-success", icon: CheckCircle },
  CANCELADA: { color: "bg-destructive/20 text-destructive", icon: XCircle },
}

interface OrdenForm {
  vehiculoId: number
  descripcionProblema: string
  diagnostico: string
  kilometrajeIngreso: number
  fechaEstimadaEntrega: string
  estadoId: number
}

export default function OrdenesPage() {
  const { user, hasRole } = useAuth()
  const [ordenes, setOrdenes] = useState<OrdenServicio[]>([])
  const [estados, setEstados] = useState<EstadoOrden[]>([])
  const [vehiculos, setVehiculos] = useState<Vehiculo[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [filterEstado, setFilterEstado] = useState<string>("all")
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [isViewOpen, setIsViewOpen] = useState(false)
  const [isEditOpen, setIsEditOpen] = useState(false)
  const [selectedOrden, setSelectedOrden] = useState<OrdenServicio | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")
  const [currentPage, setCurrentPage] = useState(1)
  const pageSize = 10

  const [formData, setFormData] = useState<OrdenForm>({
    vehiculoId: 0,
    descripcionProblema: "",
    diagnostico: "",
    kilometrajeIngreso: 0,
    fechaEstimadaEntrega: "",
    estadoId: 1,
  })

  const enrichOrden = (
    orden: OrdenServicio,
    vehiculosData = vehiculos,
    estadosData = estados
  ): OrdenServicio => ({
    ...orden,
    vehiculo: vehiculosData.find((item) => item.id === orden.vehiculoId),
    estado: estadosData.find((item) => item.id === orden.estadoId),
  })

  const getEstadoNombre = (orden: OrdenServicio) =>
    orden.estado?.nombre ||
    estados.find((estado) => estado.id === orden.estadoId)?.nombre ||
    `Estado ${orden.estadoId}`

  const getEstadoConfig = (nombre: string) =>
    estadoConfig[nombre] || estadoConfig[nombre.toUpperCase()] || estadoConfig.Pendiente

  const isEstadoTerminalNombre = (nombre: string) => {
    const normalized = nombre.toLowerCase()
    return (
      normalized.includes("complet") ||
      normalized.includes("entreg") ||
      normalized.includes("cancel") ||
      normalized === "lista"
    )
  }

  const isEstadoTerminal = (orden: OrdenServicio) => {
    return isEstadoTerminalNombre(getEstadoNombre(orden))
  }

  const estadosDisponiblesParaEdicion = estados.filter(
    (estado) => hasRole("Admin") || !isEstadoTerminalNombre(estado.nombre)
  )

  const loadOrdenes = async () => {
    setIsLoading(true)
    try {
      const [clientesResponse, marcasResponse, modelosResponse, vehiculosResponse, estadosResponse, ordenesResponse] =
        await Promise.allSettled([
          clienteService.getAll({ pageNumber: 1, pageSize: 100 }),
          vehiculoService.getMarcas(),
          vehiculoService.getModelos(),
          vehiculoService.getAll({ pageNumber: 1, pageSize: 100 }),
          ordenService.getEstados(),
          ordenService.getAll({ pageNumber: 1, pageSize: 100 }),
        ])

      const clientesData = clientesResponse.status === "fulfilled" ? clientesResponse.value.data : []
      const marcasData = marcasResponse.status === "fulfilled" ? marcasResponse.value : []
      const modelosData = modelosResponse.status === "fulfilled" ? modelosResponse.value : []
      const vehiculosBase = vehiculosResponse.status === "fulfilled" ? vehiculosResponse.value.data : []
      const estadosData = estadosResponse.status === "fulfilled" ? estadosResponse.value : []
      const ordenesData = ordenesResponse.status === "fulfilled" ? ordenesResponse.value.data : []

      const vehiculosData = vehiculosBase.map((vehiculo) => {
        const modelo = modelosData.find((item) => item.id === vehiculo.modeloId)
        const marca = marcasData.find((item) => item.id === modelo?.marcaId)

        return {
          ...vehiculo,
          cliente: clientesData.find((item) => item.id === vehiculo.clienteId),
          modelo,
          marca,
          marcaId: modelo?.marcaId || vehiculo.marcaId || 0,
        }
      })

      setVehiculos(vehiculosData)
      setEstados(estadosData)
      setOrdenes(
        ordenesData.map((orden) =>
          enrichOrden(orden, vehiculosData, estadosData)
        )
      )
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadOrdenes()
  }, [])

  const filteredOrdenes = ordenes.filter((orden) => {
    const text = searchTerm.toLowerCase()
    const matchesSearch =
      (orden.vehiculo?.placa || "").toLowerCase().includes(text) ||
      (orden.vehiculo?.cliente?.nombre || "").toLowerCase().includes(text) ||
      (orden.vehiculo?.cliente?.apellido || "").toLowerCase().includes(text) ||
      (orden.descripcionProblema || "").toLowerCase().includes(text) ||
      orden.id.toString().includes(searchTerm)
    
    const matchesEstado = filterEstado === "all" || getEstadoNombre(orden) === filterEstado

    return matchesSearch && matchesEstado
  })

  const totalPages = Math.ceil(filteredOrdenes.length / pageSize)
  const paginatedOrdenes = filteredOrdenes.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  )

  const ordenesStats = {
    total: ordenes.length,
    pendientes: ordenes.filter((o) => {
      const nombre = getEstadoNombre(o).toLowerCase()
      return nombre.includes("recib") || nombre.includes("pend")
    }).length,
    enProceso: ordenes.filter((o) => {
      const nombre = getEstadoNombre(o).toLowerCase()
      return nombre.includes("diagn") || nombre.includes("repar") || nombre.includes("proceso")
    }).length,
    completadas: ordenes.filter((o) => {
      const nombre = getEstadoNombre(o).toLowerCase()
      return nombre.includes("lista") || nombre.includes("complet") || nombre.includes("entreg")
    }).length,
  }

  const handleCreate = () => {
    setErrorMessage("")
    setFormData({
      vehiculoId: 0,
      descripcionProblema: "",
      diagnostico: "",
      kilometrajeIngreso: 0,
      fechaEstimadaEntrega: "",
      estadoId: 1,
    })
    setIsCreateOpen(true)
  }

  const handleView = (orden: OrdenServicio) => {
    setSelectedOrden(orden)
    setIsViewOpen(true)
  }

  const handleEdit = (orden: OrdenServicio) => {
    setErrorMessage("")
    setSelectedOrden(orden)
    setFormData({
      vehiculoId: orden.vehiculoId,
      descripcionProblema: orden.descripcionProblema,
      diagnostico: orden.diagnostico || "",
      kilometrajeIngreso: orden.kilometrajeIngreso,
      fechaEstimadaEntrega: orden.fechaEstimadaEntrega?.split("T")[0] || "",
      estadoId: orden.estadoId,
    })
    setIsEditOpen(true)
  }

  const handleSaveCreate = async () => {
    setIsLoading(true)
    setErrorMessage("")
    try {
      const estadoInicial = estados[0]
      const newOrden = await ordenService.create({
        vehiculoId: formData.vehiculoId,
        recepcionistaId: user?.id,
        estadoId: estadoInicial?.id || 1,
        citaId: undefined,
        kilometrajeIngreso: formData.kilometrajeIngreso,
        fechaIngreso: new Date().toISOString().split("T")[0],
        fechaEstimada: formData.fechaEstimadaEntrega,
        descripcionProblema: formData.descripcionProblema,
        observaciones: formData.descripcionProblema,
      })
      setOrdenes([enrichOrden(newOrden), ...ordenes])
      setIsCreateOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo crear la orden.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleSaveEdit = async () => {
    if (!selectedOrden) return
    setIsLoading(true)
    setErrorMessage("")
    try {
      const updatedOrden = await ordenService.updateFull(selectedOrden.id, {
        vehiculoId: formData.vehiculoId,
        recepcionistaId: user?.id || 0,
        estadoId: formData.estadoId,
        citaId: selectedOrden.citaId ?? null,
        kilometrajeIngreso: formData.kilometrajeIngreso,
        fechaEstimada: formData.fechaEstimadaEntrega || undefined,
        fechaEntregaReal: selectedOrden.fechaEntregaReal || null,
        observaciones: formData.descripcionProblema,
      })

      setOrdenes(
        ordenes.map((o) =>
          o.id === selectedOrden.id ? enrichOrden(updatedOrden) : o
        )
      )
      setIsEditOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo actualizar la orden.")
    } finally {
      setIsLoading(false)
    }
  }

  const formatDate = (dateString?: string) => {
    if (!dateString) return "No especificada"
    return new Date(dateString).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    })
  }

  const formatShortDate = (dateString?: string) => {
    if (!dateString) return "-"
    return new Date(dateString).toLocaleDateString("es-ES", {
      month: "short",
      day: "numeric",
    })
  }

  return (
    <AppLayout>
      <Header title="Órdenes de Servicio" subtitle="Gestión de trabajos del taller" />

      <div className="p-6 space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <ClipboardList className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total</p>
                <p className="text-xl font-bold">{ordenesStats.total}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-warning/10">
                <Clock className="w-5 h-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Pendientes</p>
                <p className="text-xl font-bold">{ordenesStats.pendientes}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Wrench className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">En Proceso</p>
                <p className="text-xl font-bold">{ordenesStats.enProceso}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <CheckCircle className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Completadas</p>
                <p className="text-xl font-bold">{ordenesStats.completadas}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Toolbar */}
        <div className="flex flex-col sm:flex-row gap-4 justify-between">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative w-full sm:w-80">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por placa, cliente, problema..."
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value)
                  setCurrentPage(1)
                }}
                className="pl-9 bg-secondary border-border"
              />
            </div>
            <Select value={filterEstado} onValueChange={(value) => {
              setFilterEstado(value)
              setCurrentPage(1)
            }}>
              <SelectTrigger className="w-full sm:w-44 bg-secondary border-border">
                <SelectValue placeholder="Filtrar estado" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos los estados</SelectItem>
                {estados.map((estado) => (
                  <SelectItem key={estado.id} value={estado.nombre}>
                    {estado.nombre}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <Button onClick={handleCreate} className="bg-primary text-primary-foreground">
            <Plus className="w-4 h-4 mr-2" />
            Nueva Orden
          </Button>
        </div>

        {/* Table */}
        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead className="text-muted-foreground">Orden</TableHead>
                  <TableHead className="text-muted-foreground">Vehículo</TableHead>
                  <TableHead className="text-muted-foreground">Cliente</TableHead>
                  <TableHead className="text-muted-foreground">Problema</TableHead>
                  <TableHead className="text-muted-foreground">Ingreso</TableHead>
                  <TableHead className="text-muted-foreground">Estado</TableHead>
                  <TableHead className="text-muted-foreground text-right">Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedOrdenes.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                      No se encontraron órdenes
                    </TableCell>
                  </TableRow>
                ) : (
                  paginatedOrdenes.map((orden) => {
                    const estadoNombre = getEstadoNombre(orden)
                    const config = getEstadoConfig(estadoNombre)
                    const StatusIcon = config.icon
                    return (
                      <TableRow key={orden.id} className="border-border">
                        <TableCell>
                          <span className="font-mono font-semibold text-primary">
                            ORD-{orden.id.toString().padStart(3, "0")}
                          </span>
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Car className="w-4 h-4 text-muted-foreground" />
                            <div>
                              <p className="font-medium">
                                {orden.vehiculo?.marca?.nombre} {orden.vehiculo?.modelo?.nombre}
                              </p>
                              <p className="text-sm text-muted-foreground font-mono">
                                {orden.vehiculo?.placa}
                              </p>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <p className="font-medium">
                            {orden.vehiculo?.cliente?.nombre} {orden.vehiculo?.cliente?.apellido}
                          </p>
                        </TableCell>
                        <TableCell>
                          <p className="truncate max-w-[200px]" title={orden.descripcionProblema}>
                            {orden.descripcionProblema}
                          </p>
                        </TableCell>
                        <TableCell className="text-muted-foreground">
                          {formatShortDate(orden.fechaIngreso)}
                        </TableCell>
                        <TableCell>
                          <Badge className={`${config.color} flex items-center gap-1 w-fit`}>
                            <StatusIcon className="w-3 h-3" />
                            {estadoNombre}
                          </Badge>
                        </TableCell>
                        <TableCell className="text-right">
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="ghost" size="icon">
                                <MoreHorizontal className="w-4 h-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem onClick={() => handleView(orden)}>
                                <Eye className="w-4 h-4 mr-2" />
                                Ver detalles
                              </DropdownMenuItem>
                              {!isEstadoTerminal(orden) && (
                                <DropdownMenuItem onClick={() => handleEdit(orden)}>
                                  <Pencil className="w-4 h-4 mr-2" />
                                  Editar
                                </DropdownMenuItem>
                              )}
                              <DropdownMenuItem>
                                <Receipt className="w-4 h-4 mr-2" />
                                Generar factura
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </TableCell>
                      </TableRow>
                    )
                  })
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between">
            <p className="text-sm text-muted-foreground">
              Mostrando {(currentPage - 1) * pageSize + 1} -{" "}
              {Math.min(currentPage * pageSize, filteredOrdenes.length)} de{" "}
              {filteredOrdenes.length} órdenes
            </p>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="icon"
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                disabled={currentPage === 1}
              >
                <ChevronLeft className="w-4 h-4" />
              </Button>
              <span className="text-sm text-muted-foreground">
                Página {currentPage} de {totalPages}
              </span>
              <Button
                variant="outline"
                size="icon"
                onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                disabled={currentPage === totalPages}
              >
                <ChevronRight className="w-4 h-4" />
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Create Dialog */}
      <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nueva Orden de Servicio</DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Vehículo</label>
              <Select
                value={formData.vehiculoId.toString()}
                onValueChange={(value) => {
                  const vehiculo = vehiculos.find((v) => v.id === parseInt(value))
                  setFormData({
                    ...formData,
                    vehiculoId: parseInt(value),
                    kilometrajeIngreso: vehiculo?.kilometraje || 0,
                  })
                }}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar vehículo" />
                </SelectTrigger>
                <SelectContent>
                  {vehiculos.map((vehiculo) => (
                    <SelectItem key={vehiculo.id} value={vehiculo.id.toString()}>
                      {vehiculo.placa} - {vehiculo.marca?.nombre} {vehiculo.modelo?.nombre} ({vehiculo.cliente?.nombre} {vehiculo.cliente?.apellido})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Descripción del problema</label>
              <Textarea
                value={formData.descripcionProblema}
                onChange={(e) =>
                  setFormData({ ...formData, descripcionProblema: e.target.value })
                }
                placeholder="Describa el problema o servicio requerido..."
                className="bg-secondary border-border min-h-[100px]"
              />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Kilometraje actual</label>
                <Input
                  type="number"
                  value={formData.kilometrajeIngreso}
                  onChange={(e) =>
                    setFormData({ ...formData, kilometrajeIngreso: parseInt(e.target.value) || 0 })
                  }
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Fecha estimada entrega</label>
                <Input
                  type="date"
                  value={formData.fechaEstimadaEntrega}
                  onChange={(e) =>
                    setFormData({ ...formData, fechaEstimadaEntrega: e.target.value })
                  }
                  className="bg-secondary border-border"
                />
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCreateOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleSaveCreate}
              disabled={isLoading || !formData.vehiculoId || !formData.descripcionProblema}
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Crear Orden
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Edit Dialog */}
      <Dialog open={isEditOpen} onOpenChange={setIsEditOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Editar Orden ORD-{selectedOrden?.id.toString().padStart(3, "0")}</DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Estado</label>
              <Select
                value={formData.estadoId.toString()}
                onValueChange={(value) =>
                  setFormData({ ...formData, estadoId: parseInt(value) })
                }
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {estadosDisponiblesParaEdicion.map((estado) => (
                    <SelectItem key={estado.id} value={estado.id.toString()}>
                      {estado.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Diagnóstico</label>
              <Textarea
                value={formData.diagnostico}
                onChange={(e) =>
                  setFormData({ ...formData, diagnostico: e.target.value })
                }
                placeholder="Ingrese el diagnóstico del vehículo..."
                className="bg-secondary border-border min-h-[100px]"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Fecha estimada entrega</label>
              <Input
                type="date"
                value={formData.fechaEstimadaEntrega}
                onChange={(e) =>
                  setFormData({ ...formData, fechaEstimadaEntrega: e.target.value })
                }
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsEditOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleSaveEdit}
              disabled={isLoading}
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar cambios
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* View Dialog */}
      <Dialog open={isViewOpen} onOpenChange={setIsViewOpen}>
        <DialogContent className="bg-card border-border max-w-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <ClipboardList className="w-5 h-5 text-primary" />
              Orden ORD-{selectedOrden?.id.toString().padStart(3, "0")}
            </DialogTitle>
          </DialogHeader>
          {selectedOrden && (
            <Tabs defaultValue="general" className="w-full">
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="general">General</TabsTrigger>
                <TabsTrigger value="detalles">Detalles</TabsTrigger>
              </TabsList>
              <TabsContent value="general" className="space-y-4 pt-4">
                <div className="flex items-center justify-between">
                  <Badge className={getEstadoConfig(getEstadoNombre(selectedOrden)).color}>
                    {getEstadoNombre(selectedOrden)}
                  </Badge>
                  <span className="text-sm text-muted-foreground">
                    Ingreso: {formatDate(selectedOrden.fechaIngreso)}
                  </span>
                </div>
                
                <div className="grid grid-cols-2 gap-4 p-4 rounded-lg bg-secondary/50">
                  <div className="flex items-center gap-3">
                    <Car className="w-5 h-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Vehículo</p>
                      <p className="font-medium">
                        {selectedOrden.vehiculo?.marca?.nombre} {selectedOrden.vehiculo?.modelo?.nombre}
                      </p>
                      <p className="text-sm text-muted-foreground font-mono">
                        {selectedOrden.vehiculo?.placa}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <User className="w-5 h-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Cliente</p>
                      <p className="font-medium">
                        {selectedOrden.vehiculo?.cliente?.nombre} {selectedOrden.vehiculo?.cliente?.apellido}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {selectedOrden.vehiculo?.cliente?.telefono}
                      </p>
                    </div>
                  </div>
                </div>

                <div className="space-y-3">
                  <div>
                    <p className="text-sm text-muted-foreground mb-1">Problema reportado</p>
                    <p className="p-3 rounded-lg bg-secondary/50">
                      {selectedOrden.descripcionProblema}
                    </p>
                  </div>
                  {selectedOrden.diagnostico && (
                    <div>
                      <p className="text-sm text-muted-foreground mb-1">Diagnóstico</p>
                      <p className="p-3 rounded-lg bg-secondary/50">
                        {selectedOrden.diagnostico}
                      </p>
                    </div>
                  )}
                </div>

                <div className="grid grid-cols-3 gap-4 pt-4 border-t border-border">
                  <div>
                    <p className="text-sm text-muted-foreground">Km de ingreso</p>
                    <p className="font-semibold">{selectedOrden.kilometrajeIngreso.toLocaleString()} km</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Entrega estimada</p>
                    <p className="font-semibold">{formatShortDate(selectedOrden.fechaEstimadaEntrega)}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Entrega real</p>
                    <p className="font-semibold">{formatShortDate(selectedOrden.fechaEntrega)}</p>
                  </div>
                </div>
              </TabsContent>
              <TabsContent value="detalles" className="space-y-4 pt-4">
                <div className="text-center py-8 text-muted-foreground">
                  <FileText className="w-12 h-12 mx-auto mb-2 opacity-50" />
                  <p>No hay detalles de trabajo registrados aún.</p>
                  <Button variant="outline" className="mt-4">
                    <Plus className="w-4 h-4 mr-2" />
                    Agregar detalle
                  </Button>
                </div>
              </TabsContent>
            </Tabs>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsViewOpen(false)}>
              Cerrar
            </Button>
            {selectedOrden && !isEstadoTerminal(selectedOrden) && (
              <Button
                onClick={() => {
                  setIsViewOpen(false)
                  if (selectedOrden) handleEdit(selectedOrden)
                }}
                className="bg-primary text-primary-foreground"
              >
                <Pencil className="w-4 h-4 mr-2" />
                Editar
              </Button>
            )}
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
