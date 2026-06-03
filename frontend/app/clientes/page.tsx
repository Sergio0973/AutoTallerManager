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
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
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
  Phone,
  Mail,
  MapPin,
  ChevronLeft,
  ChevronRight,
  Users,
  Loader2
} from "lucide-react"
import { clienteService } from "@/lib/api"
import type { Cliente, ClienteCreate } from "@/lib/api/types"

// Datos de ejemplo (se reemplazarán con llamadas a la API)
const mockClientes: Cliente[] = [
  {
    id: 1,
    nombre: "Carlos",
    apellido: "Mendoza",
    correo: "carlos.mendoza@email.com",
    telefono: "+1 555-0101",
    direccion: "Av. Principal 123, Ciudad",
    fechaRegistro: "2024-01-15T10:00:00Z",
    activo: true,
  },
  {
    id: 2,
    nombre: "María",
    apellido: "García",
    correo: "maria.garcia@email.com",
    telefono: "+1 555-0102",
    direccion: "Calle Secundaria 456",
    fechaRegistro: "2024-02-20T14:30:00Z",
    activo: true,
  },
  {
    id: 3,
    nombre: "Juan",
    apellido: "Pérez",
    correo: "juan.perez@email.com",
    telefono: "+1 555-0103",
    direccion: "Boulevard Norte 789",
    fechaRegistro: "2024-03-10T09:15:00Z",
    activo: true,
  },
  {
    id: 4,
    nombre: "Ana",
    apellido: "Rodríguez",
    correo: "ana.rodriguez@email.com",
    telefono: "+1 555-0104",
    direccion: "Plaza Central 321",
    fechaRegistro: "2024-03-25T16:45:00Z",
    activo: false,
  },
  {
    id: 5,
    nombre: "Luis",
    apellido: "Martínez",
    correo: "luis.martinez@email.com",
    telefono: "+1 555-0105",
    direccion: "Residencial Sur 654",
    fechaRegistro: "2024-04-05T11:20:00Z",
    activo: true,
  },
]

export default function ClientesPage() {
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [isEditOpen, setIsEditOpen] = useState(false)
  const [isViewOpen, setIsViewOpen] = useState(false)
  const [isDeleteOpen, setIsDeleteOpen] = useState(false)
  const [selectedCliente, setSelectedCliente] = useState<Cliente | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [currentPage, setCurrentPage] = useState(1)
  const pageSize = 10

  // Form state
  const [formData, setFormData] = useState<ClienteCreate>({
    nombre: "",
    apellido: "",
    documento: "",
    correo: "",
    telefono: "",
    direccion: "",
  })

  const loadClientes = async () => {
    setIsLoading(true)
    try {
      const response = await clienteService.getAll({
        pageNumber: 1,
        pageSize: 100,
        search: searchTerm || undefined,
      })
      setClientes(response.data)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadClientes()
  }, [])

  // Filtrar clientes por búsqueda
  const filteredClientes = clientes.filter(
    (cliente) =>
      cliente.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      cliente.apellido.toLowerCase().includes(searchTerm.toLowerCase()) ||
      cliente.documento?.includes(searchTerm) ||
      cliente.correo?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      cliente.telefono?.includes(searchTerm)
  )

  // Paginación
  const totalPages = Math.ceil(filteredClientes.length / pageSize)
  const paginatedClientes = filteredClientes.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  )

  const handleCreate = () => {
    setFormData({
      nombre: "",
      apellido: "",
      documento: "",
      correo: "",
      telefono: "",
      direccion: "",
    })
    setIsCreateOpen(true)
  }

  const handleEdit = (cliente: Cliente) => {
    setSelectedCliente(cliente)
    setFormData({
      nombre: cliente.nombre,
      apellido: cliente.apellido,
      documento: cliente.documento || cliente.telefono || "",
      correo: cliente.correo,
      telefono: cliente.telefono,
      direccion: cliente.direccion || "",
    })
    setIsEditOpen(true)
  }

  const handleView = (cliente: Cliente) => {
    setSelectedCliente(cliente)
    setIsViewOpen(true)
  }

  const handleDeleteClick = (cliente: Cliente) => {
    setSelectedCliente(cliente)
    setIsDeleteOpen(true)
  }

  const handleSaveCreate = async () => {
    setIsLoading(true)
    try {
      const newCliente = await clienteService.create(formData)
      setClientes([...clientes, newCliente])
      setIsCreateOpen(false)
    } finally {
      setIsLoading(false)
    }
  }

  const handleSaveEdit = async () => {
    if (!selectedCliente) return
    setIsLoading(true)
    try {
      const updatedCliente = await clienteService.update(selectedCliente.id, formData)
      setClientes(
        clientes.map((c) =>
          c.id === selectedCliente.id ? updatedCliente : c
        )
      )
      setIsEditOpen(false)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!selectedCliente) return
    setIsLoading(true)
    try {
      await clienteService.delete(selectedCliente.id)
      setClientes(clientes.filter((c) => c.id !== selectedCliente.id))
      setIsDeleteOpen(false)
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

  return (
    <AppLayout>
      <Header title="Clientes" subtitle="Gestión de clientes del taller" />

      <div className="p-6 space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Users className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Clientes</p>
                <p className="text-xl font-bold">{clientes.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <Users className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Activos</p>
                <p className="text-xl font-bold">
                  {clientes.filter((c) => c.activo).length}
                </p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-muted">
                <Users className="w-5 h-5 text-muted-foreground" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Inactivos</p>
                <p className="text-xl font-bold">
                  {clientes.filter((c) => !c.activo).length}
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
              placeholder="Buscar por nombre o documento..."
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
            Nuevo Cliente
          </Button>
        </div>

        {/* Table */}
        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead className="text-muted-foreground">Nombre</TableHead>
                  <TableHead className="text-muted-foreground">Contacto</TableHead>
                  <TableHead className="text-muted-foreground">Dirección</TableHead>
                  <TableHead className="text-muted-foreground">Registro</TableHead>
                  <TableHead className="text-muted-foreground">Estado</TableHead>
                  <TableHead className="text-muted-foreground text-right">Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedClientes.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                      No se encontraron clientes
                    </TableCell>
                  </TableRow>
                ) : (
                  paginatedClientes.map((cliente) => (
                    <TableRow key={cliente.id} className="border-border">
                      <TableCell>
                        <div className="font-medium">
                          {cliente.nombre} {cliente.apellido}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex flex-col gap-1">
                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <Mail className="w-3 h-3" />
                            {cliente.correo || "Sin correo"}
                          </div>
                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <Phone className="w-3 h-3" />
                            {cliente.documento || cliente.telefono}
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <MapPin className="w-3 h-3 flex-shrink-0" />
                          <span className="truncate max-w-[200px]">
                            {cliente.direccion || "No especificada"}
                          </span>
                        </div>
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {formatDate(cliente.fechaRegistro)}
                      </TableCell>
                      <TableCell>
                        <Badge
                          variant={cliente.activo ? "default" : "secondary"}
                          className={
                            cliente.activo
                              ? "bg-success/20 text-success hover:bg-success/30"
                              : "bg-muted text-muted-foreground"
                          }
                        >
                          {cliente.activo ? "Activo" : "Inactivo"}
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
                            <DropdownMenuItem onClick={() => handleView(cliente)}>
                              <Eye className="w-4 h-4 mr-2" />
                              Ver detalles
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => handleEdit(cliente)}>
                              <Pencil className="w-4 h-4 mr-2" />
                              Editar
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              onClick={() => handleDeleteClick(cliente)}
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
              {Math.min(currentPage * pageSize, filteredClientes.length)} de{" "}
              {filteredClientes.length} clientes
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
        <DialogContent className="bg-card border-border">
          <DialogHeader>
            <DialogTitle>Nuevo Cliente</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Nombre</label>
                <Input
                  value={formData.nombre}
                  onChange={(e) =>
                    setFormData({ ...formData, nombre: e.target.value })
                  }
                  placeholder="Nombre"
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Apellido</label>
                <Input
                  value={formData.apellido}
                  onChange={(e) =>
                    setFormData({ ...formData, apellido: e.target.value })
                  }
                  placeholder="Apellido"
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Documento</label>
              <Input
                value={formData.documento}
                onChange={(e) =>
                  setFormData({ ...formData, documento: e.target.value })
                }
                placeholder="Numero de documento"
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Teléfono</label>
              <Input
                value={formData.telefono}
                onChange={(e) =>
                  setFormData({ ...formData, telefono: e.target.value })
                }
                placeholder="+1 555-0100"
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Dirección</label>
              <Input
                value={formData.direccion}
                onChange={(e) =>
                  setFormData({ ...formData, direccion: e.target.value })
                }
                placeholder="Dirección completa"
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCreateOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleSaveCreate}
              disabled={isLoading || !formData.nombre || !formData.apellido || !formData.documento}
              className="bg-primary text-primary-foreground"
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Edit Dialog */}
      <Dialog open={isEditOpen} onOpenChange={setIsEditOpen}>
        <DialogContent className="bg-card border-border">
          <DialogHeader>
            <DialogTitle>Editar Cliente</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Nombre</label>
                <Input
                  value={formData.nombre}
                  onChange={(e) =>
                    setFormData({ ...formData, nombre: e.target.value })
                  }
                  placeholder="Nombre"
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Apellido</label>
                <Input
                  value={formData.apellido}
                  onChange={(e) =>
                    setFormData({ ...formData, apellido: e.target.value })
                  }
                  placeholder="Apellido"
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Documento</label>
              <Input
                value={formData.documento}
                onChange={(e) =>
                  setFormData({ ...formData, documento: e.target.value })
                }
                placeholder="Numero de documento"
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Teléfono</label>
              <Input
                value={formData.telefono}
                onChange={(e) =>
                  setFormData({ ...formData, telefono: e.target.value })
                }
                placeholder="+1 555-0100"
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Dirección</label>
              <Input
                value={formData.direccion}
                onChange={(e) =>
                  setFormData({ ...formData, direccion: e.target.value })
                }
                placeholder="Dirección completa"
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
              disabled={isLoading || !formData.nombre || !formData.apellido || !formData.documento}
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
        <DialogContent className="bg-card border-border">
          <DialogHeader>
            <DialogTitle>Detalles del Cliente</DialogTitle>
          </DialogHeader>
          {selectedCliente && (
            <div className="space-y-4 py-4">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-16 h-16 rounded-full bg-primary/20 text-primary text-xl font-bold">
                  {selectedCliente.nombre[0]}
                  {selectedCliente.apellido[0]}
                </div>
                <div>
                  <h3 className="text-lg font-semibold">
                    {selectedCliente.nombre} {selectedCliente.apellido}
                  </h3>
                  <Badge
                    className={
                      selectedCliente.activo
                        ? "bg-success/20 text-success"
                        : "bg-muted text-muted-foreground"
                    }
                  >
                    {selectedCliente.activo ? "Activo" : "Inactivo"}
                  </Badge>
                </div>
              </div>
              <div className="space-y-3 pt-4 border-t border-border">
                <div className="flex items-center gap-3">
                  <Mail className="w-4 h-4 text-muted-foreground" />
                  <span>{selectedCliente.correo || "Sin correo"}</span>
                </div>
                <div className="flex items-center gap-3">
                  <Phone className="w-4 h-4 text-muted-foreground" />
                  <span>{selectedCliente.documento || selectedCliente.telefono}</span>
                </div>
                <div className="flex items-center gap-3">
                  <MapPin className="w-4 h-4 text-muted-foreground" />
                  <span>{selectedCliente.direccion || "No especificada"}</span>
                </div>
              </div>
              <div className="pt-4 border-t border-border text-sm text-muted-foreground">
                Cliente desde: {formatDate(selectedCliente.fechaRegistro)}
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
                if (selectedCliente) handleEdit(selectedCliente)
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
            <AlertDialogTitle>¿Eliminar cliente?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta acción no se puede deshacer. Se eliminará permanentemente el
              cliente{" "}
              <span className="font-semibold text-foreground">
                {selectedCliente?.nombre} {selectedCliente?.apellido}
              </span>{" "}
              y todos sus datos asociados.
            </AlertDialogDescription>
          </AlertDialogHeader>
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
