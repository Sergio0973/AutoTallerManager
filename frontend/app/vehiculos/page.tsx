"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
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
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { 
  Search, 
  Plus, 
  MoreHorizontal, 
  Pencil, 
  Trash2, 
  Eye,
  Car,
  User,
  Gauge,
  Calendar,
  ChevronLeft,
  ChevronRight,
  Loader2
} from "lucide-react"
import { clienteService, vehiculoService } from "@/lib/api"
import type { Vehiculo, Cliente, Marca, Modelo } from "@/lib/api/types"

// Datos de ejemplo
const mockMarcas: Marca[] = [
  { id: 1, nombre: "Toyota" },
  { id: 2, nombre: "Honda" },
  { id: 3, nombre: "Ford" },
  { id: 4, nombre: "Chevrolet" },
  { id: 5, nombre: "Nissan" },
]

const mockModelos: Modelo[] = [
  { id: 1, nombre: "Corolla", marcaId: 1 },
  { id: 2, nombre: "Camry", marcaId: 1 },
  { id: 3, nombre: "RAV4", marcaId: 1 },
  { id: 4, nombre: "Civic", marcaId: 2 },
  { id: 5, nombre: "Accord", marcaId: 2 },
  { id: 6, nombre: "CR-V", marcaId: 2 },
  { id: 7, nombre: "Focus", marcaId: 3 },
  { id: 8, nombre: "Mustang", marcaId: 3 },
  { id: 9, nombre: "Cruze", marcaId: 4 },
  { id: 10, nombre: "Malibu", marcaId: 4 },
  { id: 11, nombre: "Sentra", marcaId: 5 },
  { id: 12, nombre: "Altima", marcaId: 5 },
]

const mockClientes: Cliente[] = [
  { id: 1, nombre: "Carlos", apellido: "Mendoza", correo: "carlos@email.com", telefono: "+1 555-0101", fechaRegistro: "2024-01-15", activo: true },
  { id: 2, nombre: "María", apellido: "García", correo: "maria@email.com", telefono: "+1 555-0102", fechaRegistro: "2024-02-20", activo: true },
  { id: 3, nombre: "Juan", apellido: "Pérez", correo: "juan@email.com", telefono: "+1 555-0103", fechaRegistro: "2024-03-10", activo: true },
]

const mockVehiculos: Vehiculo[] = [
  {
    id: 1,
    placa: "ABC-123",
    vin: "1HGBH41JXMN109186",
    color: "Blanco",
    anio: 2020,
    kilometraje: 45000,
    clienteId: 1,
    cliente: mockClientes[0],
    marcaId: 1,
    marca: mockMarcas[0],
    modeloId: 1,
    modelo: mockModelos[0],
    fechaRegistro: "2024-01-20T10:00:00Z",
  },
  {
    id: 2,
    placa: "XYZ-789",
    vin: "2HGBH41JXMN109187",
    color: "Negro",
    anio: 2019,
    kilometraje: 62000,
    clienteId: 2,
    cliente: mockClientes[1],
    marcaId: 2,
    marca: mockMarcas[1],
    modeloId: 4,
    modelo: mockModelos[3],
    fechaRegistro: "2024-02-25T14:30:00Z",
  },
  {
    id: 3,
    placa: "DEF-456",
    vin: "3HGBH41JXMN109188",
    color: "Rojo",
    anio: 2021,
    kilometraje: 28000,
    clienteId: 3,
    cliente: mockClientes[2],
    marcaId: 3,
    marca: mockMarcas[2],
    modeloId: 7,
    modelo: mockModelos[6],
    fechaRegistro: "2024-03-15T09:15:00Z",
  },
  {
    id: 4,
    placa: "GHI-321",
    vin: "4HGBH41JXMN109189",
    color: "Azul",
    anio: 2018,
    kilometraje: 85000,
    clienteId: 1,
    cliente: mockClientes[0],
    marcaId: 4,
    marca: mockMarcas[3],
    modeloId: 9,
    modelo: mockModelos[8],
    fechaRegistro: "2024-04-01T16:45:00Z",
  },
]

interface VehiculoForm {
  placa: string
  vin: string
  color: string
  anio: number
  kilometraje: number
  clienteId: number
  marcaId: number
  modeloId: number
}

export default function VehiculosPage() {
  const [vehiculos, setVehiculos] = useState<Vehiculo[]>([])
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [marcas, setMarcas] = useState<Marca[]>([])
  const [modelos, setModelos] = useState<Modelo[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [isEditOpen, setIsEditOpen] = useState(false)
  const [isViewOpen, setIsViewOpen] = useState(false)
  const [isDeleteOpen, setIsDeleteOpen] = useState(false)
  const [selectedVehiculo, setSelectedVehiculo] = useState<Vehiculo | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")
  const [currentPage, setCurrentPage] = useState(1)
  const [selectedMarcaId, setSelectedMarcaId] = useState<number | null>(null)
  const pageSize = 10

  const [formData, setFormData] = useState<VehiculoForm>({
    placa: "",
    vin: "",
    color: "",
    anio: new Date().getFullYear(),
    kilometraje: 0,
    clienteId: 0,
    marcaId: 0,
    modeloId: 0,
  })

  const enrichVehiculo = (
    vehiculo: Vehiculo,
    clientesData = clientes,
    marcasData = marcas,
    modelosData = modelos
  ): Vehiculo => {
    const modelo = modelosData.find((item) => item.id === vehiculo.modeloId)
    const marca = marcasData.find((item) => item.id === modelo?.marcaId)

    return {
      ...vehiculo,
      cliente: clientesData.find((item) => item.id === vehiculo.clienteId),
      modelo,
      marca,
      marcaId: modelo?.marcaId || vehiculo.marcaId || 0,
    }
  }

  const loadVehiculos = async () => {
    setIsLoading(true)
    try {
      const [clientesResponse, marcasResponse, modelosResponse, vehiculosResponse] = await Promise.allSettled([
        clienteService.getAll({ pageNumber: 1, pageSize: 100 }),
        vehiculoService.getMarcas(),
        vehiculoService.getModelos(),
        vehiculoService.getAll({ pageNumber: 1, pageSize: 100 }),
      ])

      const clientesData: Cliente[] = clientesResponse.status === "fulfilled" ? clientesResponse.value.data : []
      const marcasData: Marca[] = marcasResponse.status === "fulfilled" ? marcasResponse.value : []
      const modelosData: Modelo[] = modelosResponse.status === "fulfilled" ? modelosResponse.value : []
      const vehiculosData: Vehiculo[] = vehiculosResponse.status === "fulfilled" ? vehiculosResponse.value.data : []

      setClientes(clientesData)
      setMarcas(marcasData)
      setModelos(modelosData)
      setVehiculos(
        vehiculosData.map((vehiculo) =>
          enrichVehiculo(vehiculo, clientesData, marcasData, modelosData)
        )
      )
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadVehiculos()
  }, [])

  const filteredVehiculos = vehiculos.filter((v) => {
    const text = searchTerm.toLowerCase()
    return (
      (v.placa || "").toLowerCase().includes(text) ||
      (v.cliente?.nombre || "").toLowerCase().includes(text) ||
      (v.cliente?.apellido || "").toLowerCase().includes(text) ||
      (v.marca?.nombre || "").toLowerCase().includes(text) ||
      (v.modelo?.nombre || "").toLowerCase().includes(text)
    )
  })

  const totalPages = Math.ceil(filteredVehiculos.length / pageSize)
  const paginatedVehiculos = filteredVehiculos.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  )

  const filteredModelos = selectedMarcaId
    ? modelos.filter((m) => m.marcaId === selectedMarcaId)
    : modelos

  const handleCreate = () => {
    setErrorMessage("")
    setFormData({
      placa: "",
      vin: "",
      color: "",
      anio: new Date().getFullYear(),
      kilometraje: 0,
      clienteId: 0,
      marcaId: 0,
      modeloId: 0,
    })
    setSelectedMarcaId(null)
    setIsCreateOpen(true)
  }

  const handleEdit = (vehiculo: Vehiculo) => {
    setErrorMessage("")
    const modelo = modelos.find((item) => item.id === vehiculo.modeloId) || vehiculo.modelo
    const marcaId = modelo?.marcaId || vehiculo.marcaId || 0
    setSelectedVehiculo(vehiculo)
    setSelectedMarcaId(marcaId || null)
    setFormData({
      placa: vehiculo.placa || "",
      vin: vehiculo.vin || "",
      color: vehiculo.color || "",
      anio: vehiculo.anio,
      kilometraje: vehiculo.kilometraje,
      clienteId: vehiculo.clienteId,
      marcaId,
      modeloId: vehiculo.modeloId,
    })
    setIsEditOpen(true)
  }

  const handleView = (vehiculo: Vehiculo) => {
    setSelectedVehiculo(vehiculo)
    setIsViewOpen(true)
  }

  const handleDeleteClick = (vehiculo: Vehiculo) => {
    setErrorMessage("")
    setSelectedVehiculo(vehiculo)
    setIsDeleteOpen(true)
  }

  const handleSaveCreate = async () => {
    if (formData.vin.trim().length !== 17) {
      setErrorMessage("El VIN es obligatorio y debe tener exactamente 17 caracteres.")
      return
    }

    setIsLoading(true)
    setErrorMessage("")
    try {
      const newVehiculo = await vehiculoService.create(formData)
      setVehiculos([...vehiculos, enrichVehiculo(newVehiculo)])
      setIsCreateOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo crear el vehiculo.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleSaveEdit = async () => {
    if (!selectedVehiculo) return
    if (formData.vin.trim().length !== 17) {
      setErrorMessage("El VIN es obligatorio y debe tener exactamente 17 caracteres.")
      return
    }

    setIsLoading(true)
    setErrorMessage("")
    try {
      const updatedVehiculo = await vehiculoService.update(selectedVehiculo.id, formData)
      setVehiculos(
        vehiculos.map((v) =>
          v.id === selectedVehiculo.id ? enrichVehiculo(updatedVehiculo) : v
        )
      )
      setIsEditOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo actualizar el vehiculo.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!selectedVehiculo) return
    setIsLoading(true)
    setErrorMessage("")
    try {
      await vehiculoService.delete(selectedVehiculo.id)
      setVehiculos(vehiculos.filter((v) => v.id !== selectedVehiculo.id))
      setIsDeleteOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo eliminar el vehiculo.")
    } finally {
      setIsLoading(false)
    }
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "short",
      day: "numeric",
    })
  }

  const formatKm = (km: number) => {
    return km.toLocaleString("es-ES") + " km"
  }

  return (
    <AppLayout>
      <Header title="Vehículos" subtitle="Gestión de vehículos registrados" />

      <div className="p-6 space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Car className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Vehículos</p>
                <p className="text-xl font-bold">{vehiculos.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <User className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Propietarios</p>
                <p className="text-xl font-bold">
                  {new Set(vehiculos.map((v) => v.clienteId)).size}
                </p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-chart-2/10">
                <Gauge className="w-5 h-5 text-chart-2" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Prom. Kilometraje</p>
                <p className="text-xl font-bold">
                  {formatKm(
                    Math.round(
                      vehiculos.length
                        ? vehiculos.reduce((acc, v) => acc + v.kilometraje, 0) /
                          vehiculos.length
                        : 0
                    )
                  )}
                </p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-chart-3/10">
                <Calendar className="w-5 h-5 text-chart-3" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Año Promedio</p>
                <p className="text-xl font-bold">
                  {Math.round(
                    vehiculos.length
                      ? vehiculos.reduce((acc, v) => acc + v.anio, 0) / vehiculos.length
                      : 0
                  )}
                </p>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Toolbar */}
        <div className="flex flex-col sm:flex-row gap-4 justify-between">
          <div className="relative w-full sm:w-80">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por placa, cliente, marca..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value)
                setCurrentPage(1)
              }}
              className="pl-9 bg-secondary border-border"
            />
          </div>
          <Button onClick={handleCreate} className="bg-primary text-primary-foreground">
            <Plus className="w-4 h-4 mr-2" />
            Nuevo Vehículo
          </Button>
        </div>

        {/* Table */}
        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead className="text-muted-foreground">Vehículo</TableHead>
                  <TableHead className="text-muted-foreground">Propietario</TableHead>
                  <TableHead className="text-muted-foreground">Placa</TableHead>
                  <TableHead className="text-muted-foreground">Año</TableHead>
                  <TableHead className="text-muted-foreground">Kilometraje</TableHead>
                  <TableHead className="text-muted-foreground text-right">Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedVehiculos.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                      No se encontraron vehículos
                    </TableCell>
                  </TableRow>
                ) : (
                  paginatedVehiculos.map((vehiculo) => (
                    <TableRow key={vehiculo.id} className="border-border">
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-secondary">
                            <Car className="w-5 h-5 text-primary" />
                          </div>
                          <div>
                            <p className="font-medium">
                              {vehiculo.marca?.nombre} {vehiculo.modelo?.nombre}
                            </p>
                            <p className="text-sm text-muted-foreground">
                              {vehiculo.color}
                            </p>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <p className="font-medium">
                          {vehiculo.cliente?.nombre} {vehiculo.cliente?.apellido}
                        </p>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono">
                          {vehiculo.placa}
                        </Badge>
                      </TableCell>
                      <TableCell>{vehiculo.anio}</TableCell>
                      <TableCell>{formatKm(vehiculo.kilometraje)}</TableCell>
                      <TableCell className="text-right">
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="icon">
                              <MoreHorizontal className="w-4 h-4" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem onClick={() => handleView(vehiculo)}>
                              <Eye className="w-4 h-4 mr-2" />
                              Ver detalles
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => handleEdit(vehiculo)}>
                              <Pencil className="w-4 h-4 mr-2" />
                              Editar
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              onClick={() => handleDeleteClick(vehiculo)}
                              className="text-destructive focus:text-destructive"
                            >
                              <Trash2 className="w-4 h-4 mr-2" />
                              Eliminar
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))
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
              {Math.min(currentPage * pageSize, filteredVehiculos.length)} de{" "}
              {filteredVehiculos.length} vehículos
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

      {/* Create/Edit Dialog */}
      <Dialog open={isCreateOpen || isEditOpen} onOpenChange={(open) => {
        if (!open) {
          setIsCreateOpen(false)
          setIsEditOpen(false)
        }
      }}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>
              {isCreateOpen ? "Nuevo Vehículo" : "Editar Vehículo"}
            </DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Propietario</label>
              <Select
                value={formData.clienteId.toString()}
                onValueChange={(value) =>
                  setFormData({ ...formData, clienteId: parseInt(value) })
                }
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar cliente" />
                </SelectTrigger>
                <SelectContent>
                  {clientes.map((cliente) => (
                    <SelectItem key={cliente.id} value={cliente.id.toString()}>
                      {cliente.nombre} {cliente.apellido}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Marca</label>
                <Select
                  value={formData.marcaId.toString()}
                  onValueChange={(value) => {
                    const marcaId = parseInt(value)
                    setSelectedMarcaId(marcaId)
                    setFormData({ ...formData, marcaId, modeloId: 0 })
                  }}
                >
                  <SelectTrigger className="bg-secondary border-border">
                    <SelectValue placeholder="Seleccionar marca" />
                  </SelectTrigger>
                  <SelectContent>
                    {marcas.map((marca) => (
                      <SelectItem key={marca.id} value={marca.id.toString()}>
                        {marca.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Modelo</label>
                <Select
                  value={formData.modeloId.toString()}
                  onValueChange={(value) =>
                    setFormData({ ...formData, modeloId: parseInt(value) })
                  }
                  disabled={!selectedMarcaId}
                >
                  <SelectTrigger className="bg-secondary border-border">
                    <SelectValue placeholder="Seleccionar modelo" />
                  </SelectTrigger>
                  <SelectContent>
                    {filteredModelos.map((modelo) => (
                      <SelectItem key={modelo.id} value={modelo.id.toString()}>
                        {modelo.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Placa</label>
                <Input
                  value={formData.placa}
                  onChange={(e) =>
                    setFormData({ ...formData, placa: e.target.value.toUpperCase() })
                  }
                  placeholder="ABC-123"
                  className="bg-secondary border-border font-mono"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Color</label>
                <Input
                  value={formData.color}
                  onChange={(e) =>
                    setFormData({ ...formData, color: e.target.value })
                  }
                  placeholder="Color del vehículo"
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Año</label>
                <Input
                  type="number"
                  value={formData.anio}
                  onChange={(e) =>
                    setFormData({ ...formData, anio: parseInt(e.target.value) || 0 })
                  }
                  placeholder="2024"
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Kilometraje</label>
                <Input
                  type="number"
                  value={formData.kilometraje}
                  onChange={(e) =>
                    setFormData({ ...formData, kilometraje: parseInt(e.target.value) || 0 })
                  }
                  placeholder="0"
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">VIN</label>
              <Input
                maxLength={17}
                value={formData.vin}
                onChange={(e) =>
                  setFormData({ ...formData, vin: e.target.value.toUpperCase() })
                }
                placeholder="17 caracteres del VIN"
                className="bg-secondary border-border font-mono"
              />
              <p className="text-xs text-muted-foreground">
                {formData.vin.length}/17 caracteres
              </p>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => {
              setIsCreateOpen(false)
              setIsEditOpen(false)
            }}>
              Cancelar
            </Button>
            <Button
              onClick={isCreateOpen ? handleSaveCreate : handleSaveEdit}
              disabled={
                isLoading ||
                !formData.placa ||
                !formData.clienteId ||
                !formData.marcaId ||
                !formData.modeloId ||
                formData.vin.trim().length !== 17
              }
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              {isCreateOpen ? "Guardar" : "Guardar cambios"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* View Dialog */}
      <Dialog open={isViewOpen} onOpenChange={setIsViewOpen}>
        <DialogContent className="bg-card border-border">
          <DialogHeader>
            <DialogTitle>Detalles del Vehículo</DialogTitle>
          </DialogHeader>
          {selectedVehiculo && (
            <div className="space-y-4 py-4">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-16 h-16 rounded-xl bg-primary/10">
                  <Car className="w-8 h-8 text-primary" />
                </div>
                <div>
                  <h3 className="text-lg font-semibold">
                    {selectedVehiculo.marca?.nombre} {selectedVehiculo.modelo?.nombre}
                  </h3>
                  <p className="text-muted-foreground">
                    {selectedVehiculo.anio} - {selectedVehiculo.color}
                  </p>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4 pt-4 border-t border-border">
                <div>
                  <p className="text-sm text-muted-foreground">Placa</p>
                  <p className="font-mono font-semibold">{selectedVehiculo.placa}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">VIN</p>
                  <p className="font-mono text-sm">{selectedVehiculo.vin || "No registrado"}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Kilometraje</p>
                  <p className="font-semibold">{formatKm(selectedVehiculo.kilometraje)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Registrado</p>
                  <p>{formatDate(selectedVehiculo.fechaRegistro)}</p>
                </div>
              </div>
              <div className="pt-4 border-t border-border">
                <p className="text-sm text-muted-foreground mb-2">Propietario</p>
                <div className="flex items-center gap-3">
                  <div className="flex items-center justify-center w-10 h-10 rounded-full bg-secondary">
                    <User className="w-5 h-5 text-muted-foreground" />
                  </div>
                  <div>
                    <p className="font-medium">
                      {selectedVehiculo.cliente?.nombre} {selectedVehiculo.cliente?.apellido}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {selectedVehiculo.cliente?.telefono}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsViewOpen(false)}>
              Cerrar
            </Button>
            <Button
              onClick={() => {
                setIsViewOpen(false)
                if (selectedVehiculo) handleEdit(selectedVehiculo)
              }}
              className="bg-primary text-primary-foreground"
            >
              <Pencil className="w-4 h-4 mr-2" />
              Editar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation */}
      <AlertDialog open={isDeleteOpen} onOpenChange={setIsDeleteOpen}>
        <AlertDialogContent className="bg-card border-border">
          <AlertDialogHeader>
            <AlertDialogTitle>¿Eliminar vehículo?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta acción no se puede deshacer. Se eliminará permanentemente el
              vehículo{" "}
              <span className="font-semibold text-foreground">
                {selectedVehiculo?.marca?.nombre} {selectedVehiculo?.modelo?.nombre}
              </span>{" "}
              con placa{" "}
              <span className="font-mono font-semibold text-foreground">
                {selectedVehiculo?.placa}
              </span>
              .
            </AlertDialogDescription>
          </AlertDialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </AppLayout>
  )
}
