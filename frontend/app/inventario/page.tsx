"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
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
import { AlertTriangle, Loader2, Package, Plus, Search, ShoppingCart, Truck } from "lucide-react"
import { inventarioService } from "@/lib/api"
import { useAuth } from "@/contexts/auth-context"
import type {
  CategoriaRepuesto,
  Compra,
  CompraCreate,
  DetalleCompra,
  DetalleCompraCreate,
  Proveedor,
  ProveedorCreate,
  Repuesto,
  RepuestoCreate,
  UnidadMedida,
} from "@/lib/api/types"

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 }).format(value)

export default function InventarioPage() {
  const { user } = useAuth()
  const [repuestos, setRepuestos] = useState<Repuesto[]>([])
  const [categorias, setCategorias] = useState<CategoriaRepuesto[]>([])
  const [unidades, setUnidades] = useState<UnidadMedida[]>([])
  const [proveedores, setProveedores] = useState<Proveedor[]>([])
  const [compras, setCompras] = useState<Compra[]>([])
  const [detallesCompra, setDetallesCompra] = useState<DetalleCompra[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [isProveedorOpen, setIsProveedorOpen] = useState(false)
  const [isCompraOpen, setIsCompraOpen] = useState(false)
  const [isDetalleCompraOpen, setIsDetalleCompraOpen] = useState(false)
  const [selectedCompra, setSelectedCompra] = useState<Compra | null>(null)
  const [error, setError] = useState("")
  const [loadError, setLoadError] = useState("")
  const [inventoryMessage, setInventoryMessage] = useState("")
  const [formData, setFormData] = useState<RepuestoCreate>({
    categoriaId: 0,
    unidadId: 0,
    codigo: "",
    descripcion: "",
    stockActual: 0,
    stockMinimo: 1,
    precioUnitario: 0,
    precioCosto: 0,
  })
  const [proveedorForm, setProveedorForm] = useState<ProveedorCreate>({
    nombre: "",
    nit: "",
    telefono: "",
    correo: "",
    ciudadId: 2,
  })
  const [compraForm, setCompraForm] = useState<CompraCreate>({
    proveedorId: 0,
    usuarioId: 0,
    fechaCompra: new Date().toISOString().slice(0, 10),
    estado: "Recibida",
    observaciones: "",
  })
  const [detalleCompraForm, setDetalleCompraForm] = useState<DetalleCompraCreate>({
    compraId: 0,
    repuestoId: 0,
    cantidad: 1,
    precioUnitario: 0,
  })

  const loadData = async () => {
    setIsLoading(true)
    setLoadError("")
    try {
      const [
        repuestosResponse,
        categoriasResponse,
        unidadesResponse,
        proveedoresResponse,
        comprasResponse,
        detallesCompraResponse,
      ] = await Promise.allSettled([
        inventarioService.getAll({ pageNumber: 1, pageSize: 100 }),
        inventarioService.getCategorias(),
        inventarioService.getUnidadesMedida(),
        inventarioService.getProveedores(),
        inventarioService.getCompras(),
        inventarioService.getDetallesCompra(),
      ])
      setRepuestos(repuestosResponse.status === "fulfilled" ? repuestosResponse.value.data : [])
      setCategorias(categoriasResponse.status === "fulfilled" ? categoriasResponse.value : [])
      setUnidades(unidadesResponse.status === "fulfilled" ? unidadesResponse.value : [])
      setProveedores(proveedoresResponse.status === "fulfilled" ? proveedoresResponse.value : [])
      setCompras(comprasResponse.status === "fulfilled" ? comprasResponse.value : [])
      setDetallesCompra(detallesCompraResponse.status === "fulfilled" ? detallesCompraResponse.value : [])

      const failedInventoryRequests = [proveedoresResponse, comprasResponse, detallesCompraResponse].some(
        (response) => response.status === "rejected"
      )
      if (failedInventoryRequests) {
        setLoadError("No se pudieron cargar todos los datos de compras. Verifica que estes con rol Admin.")
      }
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const filteredRepuestos = repuestos.filter((repuesto) => {
    const text = searchTerm.toLowerCase()
    return (
      (repuesto.codigo || "").toLowerCase().includes(text) ||
      (repuesto.descripcion || "").toLowerCase().includes(text) ||
      (repuesto.nombre || "").toLowerCase().includes(text)
    )
  })

  const bajoStock = repuestos.filter((repuesto) => repuesto.stockActual <= repuesto.stockMinimo)
  const comprasOrdenadas = [...compras].sort((a, b) => b.id - a.id)
  const detallesCompraOrdenados = [...detallesCompra].sort((a, b) => b.id - a.id)

  const handleOpenCreate = () => {
    setError("")
    setFormData({
      categoriaId: categorias[0]?.id || 0,
      unidadId: unidades[0]?.id || 0,
      codigo: "",
      descripcion: "",
      stockActual: 0,
      stockMinimo: 1,
      precioUnitario: 0,
      precioCosto: 0,
    })
    setIsCreateOpen(true)
  }

  const handleCreate = async () => {
    setError("")
    setIsLoading(true)
    try {
      const repuesto = await inventarioService.create(formData)
      setRepuestos([repuesto, ...repuestos])
      setIsCreateOpen(false)
    } catch {
      setError("No se pudo crear el repuesto. Verifica que estes con rol Admin y que el codigo no exista.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenProveedor = () => {
    setError("")
    setInventoryMessage("")
    setProveedorForm({
      nombre: "",
      nit: "",
      telefono: "",
      correo: "",
      ciudadId: 2,
    })
    setIsProveedorOpen(true)
  }

  const handleCreateProveedor = async () => {
    setError("")
    setIsLoading(true)
    try {
      const proveedor = await inventarioService.createProveedor(proveedorForm)
      setProveedores([proveedor, ...proveedores])
      setInventoryMessage("Proveedor creado correctamente.")
      setIsProveedorOpen(false)
    } catch {
      setError("No se pudo crear el proveedor. Verifica los datos y que estes con rol Admin.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenCompra = () => {
    setError("")
    setInventoryMessage("")
    setCompraForm({
      proveedorId: proveedores[0]?.id || 0,
      usuarioId: user?.id || 0,
      fechaCompra: new Date().toISOString().slice(0, 10),
      estado: "Recibida",
      observaciones: "",
    })
    setIsCompraOpen(true)
  }

  const handleCreateCompra = async () => {
    setError("")
    setIsLoading(true)
    try {
      const compra = await inventarioService.createCompra(compraForm)
      setCompras([compra, ...compras])
      setInventoryMessage("Compra registrada correctamente. Ahora puedes agregar sus repuestos.")
      setIsCompraOpen(false)
    } catch {
      setError("No se pudo crear la compra. Verifica proveedor, usuario y rol Admin.")
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenDetalleCompra = (compra: Compra) => {
    setError("")
    setInventoryMessage("")
    setSelectedCompra(compra)
    setDetalleCompraForm({
      compraId: compra.id,
      repuestoId: repuestos[0]?.id || 0,
      cantidad: 1,
      precioUnitario: repuestos[0]?.precioUnitario ?? repuestos[0]?.precioVenta ?? 0,
    })
    setIsDetalleCompraOpen(true)
  }

  const handleCreateDetalleCompra = async () => {
    setError("")
    setIsLoading(true)
    try {
      await inventarioService.createDetalleCompra(detalleCompraForm)
      await loadData()
      setInventoryMessage("Detalle agregado correctamente. Stock y total de compra actualizados.")
      setIsDetalleCompraOpen(false)
    } catch {
      setError("No se pudo agregar el detalle de compra. Verifica repuesto, cantidad y precio.")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AppLayout>
      <Header title="Inventario" subtitle="Control de repuestos y niveles de stock" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Package className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Repuestos</p>
                <p className="text-xl font-bold">{repuestos.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-warning/10">
                <AlertTriangle className="w-5 h-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Bajo stock</p>
                <p className="text-xl font-bold">{bajoStock.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <Package className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Stock total</p>
                <p className="text-xl font-bold">{repuestos.reduce((total, repuesto) => total + repuesto.stockActual, 0)}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="flex flex-col sm:flex-row gap-4 justify-between">
          <div className="relative w-full sm:w-80">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por codigo o descripcion..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="pl-9 bg-secondary border-border"
            />
          </div>
          <Button onClick={handleOpenCreate} className="bg-primary text-primary-foreground">
            <Plus className="w-4 h-4 mr-2" />
            Nuevo Repuesto
          </Button>
        </div>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead>Codigo</TableHead>
                  <TableHead>Descripcion</TableHead>
                  <TableHead>Categoria</TableHead>
                  <TableHead>Unidad</TableHead>
                  <TableHead>Stock</TableHead>
                  <TableHead>Precio</TableHead>
                  <TableHead>Estado</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredRepuestos.map((repuesto) => {
                  const isLowStock = repuesto.stockActual <= repuesto.stockMinimo
                  return (
                    <TableRow key={repuesto.id} className="border-border">
                      <TableCell className="font-mono font-medium">{repuesto.codigo}</TableCell>
                      <TableCell>{repuesto.descripcion || repuesto.nombre}</TableCell>
                      <TableCell>{categorias.find((categoria) => categoria.id === repuesto.categoriaId)?.nombre || repuesto.categoriaId}</TableCell>
                      <TableCell>{unidades.find((unidad) => unidad.id === (repuesto.unidadId ?? repuesto.unidadMedidaId))?.nombre || repuesto.unidadId}</TableCell>
                      <TableCell>
                        <span className={isLowStock ? "text-warning font-semibold" : ""}>
                          {repuesto.stockActual} / min {repuesto.stockMinimo}
                        </span>
                      </TableCell>
                      <TableCell>{formatCurrency(repuesto.precioUnitario ?? repuesto.precioVenta)}</TableCell>
                      <TableCell>
                        <Badge className={isLowStock ? "bg-warning/20 text-warning" : "bg-success/20 text-success"}>
                          {isLowStock ? "Bajo stock" : "Disponible"}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  )
                })}
                {!isLoading && filteredRepuestos.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                      No se encontraron repuestos
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <div className="flex flex-col gap-4 border-b border-border p-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h2 className="text-lg font-semibold">Compras y proveedores</h2>
                <p className="text-sm text-muted-foreground">
                  Registra entradas de inventario sin salir del modulo.
                </p>
              </div>
              <div className="flex flex-col gap-2 sm:flex-row">
                <Button variant="outline" onClick={handleOpenProveedor}>
                  <Truck className="w-4 h-4 mr-2" />
                  Nuevo Proveedor
                </Button>
                <Button onClick={handleOpenCompra} disabled={proveedores.length === 0}>
                  <ShoppingCart className="w-4 h-4 mr-2" />
                  Nueva Compra
                </Button>
              </div>
            </div>
            {inventoryMessage && (
              <div className="mx-4 mt-4 rounded-lg border border-success/20 bg-success/10 p-3 text-sm text-success">
                {inventoryMessage}
              </div>
            )}
            {loadError && (
              <div className="mx-4 mt-4 rounded-lg border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">
                {loadError}
              </div>
            )}
            <div className="grid grid-cols-1 gap-4 p-4 md:grid-cols-3">
              <div className="rounded-lg border border-border bg-secondary/30 p-4">
                <p className="text-sm text-muted-foreground">Proveedores registrados</p>
                <p className="text-2xl font-bold">{proveedores.length}</p>
              </div>
              <div className="rounded-lg border border-border bg-secondary/30 p-4">
                <p className="text-sm text-muted-foreground">Compras registradas</p>
                <p className="text-2xl font-bold">{compras.length}</p>
              </div>
              <div className="rounded-lg border border-border bg-secondary/30 p-4">
                <p className="text-sm text-muted-foreground">Detalles de compra</p>
                <p className="text-2xl font-bold">{detallesCompra.length}</p>
              </div>
            </div>
            <div className="border-t border-border p-4">
              <h3 className="mb-3 text-sm font-semibold">Proveedores registrados</h3>
              <div className="overflow-x-auto rounded-lg border border-border">
                <Table>
                  <TableHeader>
                    <TableRow className="border-border hover:bg-transparent">
                      <TableHead>Proveedor</TableHead>
                      <TableHead>NIT</TableHead>
                      <TableHead>Telefono</TableHead>
                      <TableHead>Correo</TableHead>
                      <TableHead>Ciudad</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {proveedores.map((proveedor) => (
                      <TableRow key={proveedor.id} className="border-border">
                        <TableCell className="font-medium">{proveedor.nombre}</TableCell>
                        <TableCell>{proveedor.nit}</TableCell>
                        <TableCell>{proveedor.telefono}</TableCell>
                        <TableCell>{proveedor.correo}</TableCell>
                        <TableCell>{proveedor.ciudadId}</TableCell>
                      </TableRow>
                    ))}
                    {!isLoading && proveedores.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={5} className="text-center py-6 text-muted-foreground">
                          No hay proveedores registrados
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </div>
            </div>
            <div className="border-t border-border p-4">
              <h3 className="mb-3 text-sm font-semibold">Compras registradas</h3>
              <div className="overflow-x-auto rounded-lg border border-border">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead>Compra</TableHead>
                  <TableHead>Proveedor</TableHead>
                  <TableHead>Fecha</TableHead>
                  <TableHead>Total</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead className="text-right">Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {comprasOrdenadas.map((compra) => (
                  <TableRow key={compra.id} className="border-border">
                    <TableCell className="font-mono font-medium">COM-{compra.id.toString().padStart(3, "0")}</TableCell>
                    <TableCell>{proveedores.find((proveedor) => proveedor.id === compra.proveedorId)?.nombre || compra.proveedorId}</TableCell>
                    <TableCell>{compra.fechaCompra}</TableCell>
                    <TableCell>{formatCurrency(compra.total || 0)}</TableCell>
                    <TableCell>
                      <Badge className="bg-success/20 text-success">{compra.estado}</Badge>
                    </TableCell>
                    <TableCell className="text-right">
                      <Button variant="outline" size="sm" onClick={() => handleOpenDetalleCompra(compra)}>
                        Agregar detalle
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
                {!isLoading && comprasOrdenadas.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                      No se encontraron compras registradas
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
              </div>
            </div>
            <div className="border-t border-border p-4">
              <h3 className="mb-3 text-sm font-semibold">Detalles de compra registrados</h3>
              <div className="overflow-x-auto rounded-lg border border-border">
                <Table>
                  <TableHeader>
                    <TableRow className="border-border hover:bg-transparent">
                      <TableHead>Detalle</TableHead>
                      <TableHead>Compra</TableHead>
                      <TableHead>Repuesto</TableHead>
                      <TableHead>Cantidad</TableHead>
                      <TableHead>Precio unitario</TableHead>
                      <TableHead>Subtotal</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {detallesCompraOrdenados.map((detalle) => (
                      <TableRow key={detalle.id} className="border-border">
                        <TableCell className="font-mono font-medium">DET-{detalle.id.toString().padStart(3, "0")}</TableCell>
                        <TableCell>COM-{detalle.compraId.toString().padStart(3, "0")}</TableCell>
                        <TableCell>
                          {repuestos.find((repuesto) => repuesto.id === detalle.repuestoId)?.codigo || detalle.repuestoId}
                        </TableCell>
                        <TableCell>{detalle.cantidad}</TableCell>
                        <TableCell>{formatCurrency(detalle.precioUnitario)}</TableCell>
                        <TableCell>{formatCurrency(detalle.subtotal)}</TableCell>
                      </TableRow>
                    ))}
                    {!isLoading && detallesCompraOrdenados.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center py-6 text-muted-foreground">
                          No hay detalles de compra registrados
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nuevo Repuesto</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {error && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">
                {error}
              </div>
            )}
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Categoria</label>
                <Select
                  value={formData.categoriaId.toString()}
                  onValueChange={(value) => setFormData({ ...formData, categoriaId: Number(value) })}
                >
                  <SelectTrigger className="bg-secondary border-border">
                    <SelectValue placeholder="Seleccionar categoria" />
                  </SelectTrigger>
                  <SelectContent>
                    {categorias.map((categoria) => (
                      <SelectItem key={categoria.id} value={categoria.id.toString()}>
                        {categoria.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Unidad</label>
                <Select
                  value={(formData.unidadId || 0).toString()}
                  onValueChange={(value) => setFormData({ ...formData, unidadId: Number(value) })}
                >
                  <SelectTrigger className="bg-secondary border-border">
                    <SelectValue placeholder="Seleccionar unidad" />
                  </SelectTrigger>
                  <SelectContent>
                    {unidades.map((unidad) => (
                      <SelectItem key={unidad.id} value={unidad.id.toString()}>
                        {unidad.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Codigo</label>
                <Input
                  value={formData.codigo}
                  onChange={(event) => setFormData({ ...formData, codigo: event.target.value.toUpperCase() })}
                  placeholder="REP-001"
                  className="bg-secondary border-border font-mono"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Precio unitario</label>
                <Input
                  type="text"
                  inputMode="decimal"
                  value={formData.precioUnitario || ""}
                  onChange={(event) => {
                    const rawValue = event.target.value.replace(/[^\d.,]/g, "")
                    const normalizedValue = rawValue.replace(/\./g, "").replace(",", ".")
                    const numericValue = Number(normalizedValue) || 0
                    setFormData({
                      ...formData,
                      precioUnitario: numericValue,
                      precioCosto: numericValue,
                    })
                  }}
                  placeholder="25000"
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Descripcion</label>
              <Input
                value={formData.descripcion}
                onChange={(event) => setFormData({ ...formData, descripcion: event.target.value })}
                placeholder="Descripcion del repuesto"
                className="bg-secondary border-border"
              />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Stock actual</label>
                <Input
                  type="number"
                  value={formData.stockActual}
                  onChange={(event) => setFormData({ ...formData, stockActual: Number(event.target.value) })}
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Stock minimo</label>
                <Input
                  type="number"
                  value={formData.stockMinimo}
                  onChange={(event) => setFormData({ ...formData, stockMinimo: Number(event.target.value) })}
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
              onClick={handleCreate}
              disabled={
                isLoading ||
                !formData.categoriaId ||
                !formData.unidadId ||
                !formData.codigo ||
                !formData.descripcion ||
                !formData.precioUnitario
              }
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isProveedorOpen} onOpenChange={setIsProveedorOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nuevo Proveedor</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {error && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">
                {error}
              </div>
            )}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Nombre</label>
                <Input
                  value={proveedorForm.nombre}
                  onChange={(event) => setProveedorForm({ ...proveedorForm, nombre: event.target.value })}
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">NIT</label>
                <Input
                  value={proveedorForm.nit}
                  onChange={(event) => setProveedorForm({ ...proveedorForm, nit: event.target.value })}
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Telefono</label>
                <Input
                  value={proveedorForm.telefono}
                  onChange={(event) => setProveedorForm({ ...proveedorForm, telefono: event.target.value })}
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Ciudad ID</label>
                <Input
                  type="text"
                  inputMode="numeric"
                  value={proveedorForm.ciudadId || ""}
                  onChange={(event) => setProveedorForm({ ...proveedorForm, ciudadId: Number(event.target.value) || 0 })}
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Correo</label>
              <Input
                type="email"
                value={proveedorForm.correo}
                onChange={(event) => setProveedorForm({ ...proveedorForm, correo: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsProveedorOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreateProveedor}
              disabled={isLoading || !proveedorForm.nombre || !proveedorForm.nit || !proveedorForm.ciudadId}
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isCompraOpen} onOpenChange={setIsCompraOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nueva Compra</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {error && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">
                {error}
              </div>
            )}
            <div className="space-y-2">
              <label className="text-sm font-medium">Proveedor</label>
              <Select
                value={(compraForm.proveedorId || 0).toString()}
                onValueChange={(value) => setCompraForm({ ...compraForm, proveedorId: Number(value) })}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar proveedor" />
                </SelectTrigger>
                <SelectContent>
                  {proveedores.map((proveedor) => (
                    <SelectItem key={proveedor.id} value={proveedor.id.toString()}>
                      {proveedor.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Fecha</label>
                <Input
                  type="date"
                  value={compraForm.fechaCompra}
                  onChange={(event) => setCompraForm({ ...compraForm, fechaCompra: event.target.value })}
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Estado</label>
                <Input
                  value={compraForm.estado}
                  onChange={(event) => setCompraForm({ ...compraForm, estado: event.target.value })}
                  className="bg-secondary border-border"
                />
              </div>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Observaciones</label>
              <Textarea
                value={compraForm.observaciones}
                onChange={(event) => setCompraForm({ ...compraForm, observaciones: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCompraOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreateCompra}
              disabled={isLoading || !compraForm.proveedorId || !compraForm.usuarioId || !compraForm.fechaCompra}
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isDetalleCompraOpen} onOpenChange={setIsDetalleCompraOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Agregar Detalle de Compra</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {error && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">
                {error}
              </div>
            )}
            <div className="rounded-lg border border-border bg-secondary/40 p-3 text-sm text-muted-foreground">
              Compra seleccionada: COM-{(selectedCompra?.id || 0).toString().padStart(3, "0")}
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Repuesto</label>
              <Select
                value={(detalleCompraForm.repuestoId || 0).toString()}
                onValueChange={(value) => {
                  const repuesto = repuestos.find((item) => item.id === Number(value))
                  const precio = repuesto?.precioUnitario ?? repuesto?.precioVenta ?? 0
                  setDetalleCompraForm({ ...detalleCompraForm, repuestoId: Number(value), precioUnitario: precio })
                }}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar repuesto" />
                </SelectTrigger>
                <SelectContent>
                  {repuestos.map((repuesto) => (
                    <SelectItem key={repuesto.id} value={repuesto.id.toString()}>
                      {repuesto.codigo} - {repuesto.descripcion || repuesto.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Cantidad</label>
                <Input
                  type="text"
                  inputMode="numeric"
                  value={detalleCompraForm.cantidad || ""}
                  onChange={(event) => setDetalleCompraForm({ ...detalleCompraForm, cantidad: Number(event.target.value) || 0 })}
                  className="bg-secondary border-border"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Precio unitario</label>
                <Input
                  type="text"
                  inputMode="decimal"
                  value={detalleCompraForm.precioUnitario || ""}
                  onChange={(event) => {
                    const rawValue = event.target.value.replace(/[^\d.,]/g, "")
                    const normalizedValue = rawValue.replace(/\./g, "").replace(",", ".")
                    setDetalleCompraForm({ ...detalleCompraForm, precioUnitario: Number(normalizedValue) || 0 })
                  }}
                  className="bg-secondary border-border"
                />
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDetalleCompraOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreateDetalleCompra}
              disabled={isLoading || !detalleCompraForm.compraId || !detalleCompraForm.repuestoId || !detalleCompraForm.cantidad || !detalleCompraForm.precioUnitario}
            >
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Agregar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
