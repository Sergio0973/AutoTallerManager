"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
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
import { AlertTriangle, Package, Plus, Search } from "lucide-react"
import { inventarioService } from "@/lib/api"
import type { CategoriaRepuesto, Repuesto, UnidadMedida } from "@/lib/api/types"

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 }).format(value)

export default function InventarioPage() {
  const [repuestos, setRepuestos] = useState<Repuesto[]>([])
  const [categorias, setCategorias] = useState<CategoriaRepuesto[]>([])
  const [unidades, setUnidades] = useState<UnidadMedida[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [repuestosResponse, categoriasResponse, unidadesResponse] = await Promise.all([
        inventarioService.getAll({ pageNumber: 1, pageSize: 100 }),
        inventarioService.getCategorias(),
        inventarioService.getUnidadesMedida(),
      ])
      setRepuestos(repuestosResponse.data)
      setCategorias(categoriasResponse)
      setUnidades(unidadesResponse)
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
      repuesto.codigo.toLowerCase().includes(text) ||
      repuesto.descripcion?.toLowerCase().includes(text) ||
      repuesto.nombre.toLowerCase().includes(text)
    )
  })

  const bajoStock = repuestos.filter((repuesto) => repuesto.stockActual <= repuesto.stockMinimo)

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
          <Button variant="outline" disabled>
            <Plus className="w-4 h-4 mr-2" />
            Alta desde Swagger
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
    </AppLayout>
  )
}
