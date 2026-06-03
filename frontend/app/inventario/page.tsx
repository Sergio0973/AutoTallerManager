"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
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
import { AlertTriangle, Loader2, Package, Plus, Search } from "lucide-react"
import { inventarioService } from "@/lib/api"
import type { CategoriaRepuesto, Repuesto, RepuestoCreate, UnidadMedida } from "@/lib/api/types"

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 }).format(value)

export default function InventarioPage() {
  const [repuestos, setRepuestos] = useState<Repuesto[]>([])
  const [categorias, setCategorias] = useState<CategoriaRepuesto[]>([])
  const [unidades, setUnidades] = useState<UnidadMedida[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [error, setError] = useState("")
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

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [repuestosResponse, categoriasResponse, unidadesResponse] = await Promise.allSettled([
        inventarioService.getAll({ pageNumber: 1, pageSize: 100 }),
        inventarioService.getCategorias(),
        inventarioService.getUnidadesMedida(),
      ])
      setRepuestos(repuestosResponse.status === "fulfilled" ? repuestosResponse.value.data : [])
      setCategorias(categoriasResponse.status === "fulfilled" ? categoriasResponse.value : [])
      setUnidades(unidadesResponse.status === "fulfilled" ? unidadesResponse.value : [])
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
    </AppLayout>
  )
}
