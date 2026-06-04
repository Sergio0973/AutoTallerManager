"use client"

import { useEffect, useMemo, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
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
import { CalendarClock, Eye, Loader2, Search, ShieldCheck, UserCheck } from "lucide-react"
import { auditoriaService } from "@/lib/api"
import type { Auditoria, AuditoriaFilterParams, Usuario } from "@/lib/api/types"

const formatDateTime = (value?: string) => {
  if (!value) return "-"
  return new Date(value).toLocaleString("es-CO", {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  })
}

const prettyJson = (value?: string | null) => {
  if (!value) return "Sin datos"

  try {
    return JSON.stringify(JSON.parse(value), null, 2)
  } catch {
    return value
  }
}

const getActionClass = (action: string) => {
  const normalized = action.toLowerCase()
  if (normalized.includes("crear") || normalized.includes("create")) return "bg-success/20 text-success"
  if (normalized.includes("actualizar") || normalized.includes("update")) return "bg-primary/20 text-primary"
  if (normalized.includes("eliminar") || normalized.includes("delete")) return "bg-destructive/20 text-destructive"
  return "bg-warning/20 text-warning"
}

export default function AuditoriaPage() {
  const [auditorias, setAuditorias] = useState<Auditoria[]>([])
  const [usuarios, setUsuarios] = useState<Usuario[]>([])
  const [filters, setFilters] = useState<AuditoriaFilterParams>({})
  const [searchTerm, setSearchTerm] = useState("")
  const [selectedAudit, setSelectedAudit] = useState<Auditoria | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")

  const loadData = async (params?: AuditoriaFilterParams) => {
    setIsLoading(true)
    setErrorMessage("")
    try {
      const [auditoriasResponse, usuariosResponse] = await Promise.allSettled([
        auditoriaService.getAll(params),
        auditoriaService.getUsuarios(),
      ])

      setAuditorias(auditoriasResponse.status === "fulfilled" ? auditoriasResponse.value : [])
      setUsuarios(usuariosResponse.status === "fulfilled" ? usuariosResponse.value : [])

      if (auditoriasResponse.status === "rejected") {
        setErrorMessage("No se pudieron cargar las auditorias. Verifica que estes con rol Admin.")
      }
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const usuariosById = useMemo(
    () => new Map(usuarios.map((usuario) => [usuario.id, usuario])),
    [usuarios]
  )

  const entidades = useMemo(
    () => Array.from(new Set(auditorias.map((auditoria) => auditoria.entidad))).sort(),
    [auditorias]
  )

  const acciones = useMemo(
    () => Array.from(new Set(auditorias.map((auditoria) => auditoria.tipoAccion))).sort(),
    [auditorias]
  )

  const auditoriasFiltradas = auditorias
    .filter((auditoria) => {
      const text = searchTerm.toLowerCase()
      const usuario = usuariosById.get(auditoria.usuarioId)
      return (
        auditoria.entidad.toLowerCase().includes(text) ||
        auditoria.tipoAccion.toLowerCase().includes(text) ||
        auditoria.entidadId.toString().includes(text) ||
        auditoria.usuarioId.toString().includes(text) ||
        (usuario?.nombre || "").toLowerCase().includes(text) ||
        (usuario?.correo || "").toLowerCase().includes(text)
      )
    })
    .sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime())

  const handleApplyFilters = () => {
    loadData(filters)
  }

  const handleClearFilters = () => {
    setFilters({})
    setSearchTerm("")
    loadData()
  }

  return (
    <AppLayout>
      <Header title="Auditoria" subtitle="Consulta de acciones registradas en el sistema" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
                <ShieldCheck className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Registros</p>
                <p className="text-xl font-bold">{auditorias.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-success/10">
                <UserCheck className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Usuarios auditados</p>
                <p className="text-xl font-bold">{new Set(auditorias.map((audit) => audit.usuarioId)).size}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-warning/10">
                <CalendarClock className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Entidades</p>
                <p className="text-xl font-bold">{entidades.length}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        {errorMessage && (
          <Alert variant="destructive">
            <AlertDescription>{errorMessage}</AlertDescription>
          </Alert>
        )}

        <Card className="bg-card border-border">
          <CardContent className="p-4 space-y-4">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-5">
              <Select
                value={filters.usuarioId?.toString() || "todos"}
                onValueChange={(value) =>
                  setFilters({ ...filters, usuarioId: value === "todos" ? undefined : Number(value) })
                }
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Usuario" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos los usuarios</SelectItem>
                  {usuarios.map((usuario) => (
                    <SelectItem key={usuario.id} value={usuario.id.toString()}>
                      {usuario.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>

              <Select
                value={filters.entidad || "todas"}
                onValueChange={(value) =>
                  setFilters({ ...filters, entidad: value === "todas" ? undefined : value })
                }
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Entidad" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todas">Todas las entidades</SelectItem>
                  {entidades.map((entidad) => (
                    <SelectItem key={entidad} value={entidad}>
                      {entidad}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>

              <Select
                value={filters.tipoAccion || "todas"}
                onValueChange={(value) =>
                  setFilters({ ...filters, tipoAccion: value === "todas" ? undefined : value })
                }
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Accion" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todas">Todas las acciones</SelectItem>
                  {acciones.map((accion) => (
                    <SelectItem key={accion} value={accion}>
                      {accion}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>

              <Input
                type="date"
                value={filters.desde || ""}
                onChange={(event) => setFilters({ ...filters, desde: event.target.value || undefined })}
                className="bg-secondary border-border"
              />
              <Input
                type="date"
                value={filters.hasta || ""}
                onChange={(event) => setFilters({ ...filters, hasta: event.target.value || undefined })}
                className="bg-secondary border-border"
              />
            </div>

            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="relative w-full sm:w-80">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                <Input
                  placeholder="Buscar por entidad, usuario o accion..."
                  value={searchTerm}
                  onChange={(event) => setSearchTerm(event.target.value)}
                  className="pl-9 bg-secondary border-border"
                />
              </div>
              <div className="flex gap-2">
                <Button variant="outline" onClick={handleClearFilters} disabled={isLoading}>
                  Limpiar
                </Button>
                <Button onClick={handleApplyFilters} disabled={isLoading}>
                  {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  Aplicar filtros
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow className="border-border hover:bg-transparent">
                    <TableHead>Fecha</TableHead>
                    <TableHead>Usuario</TableHead>
                    <TableHead>Entidad</TableHead>
                    <TableHead>Entidad ID</TableHead>
                    <TableHead>Accion</TableHead>
                    <TableHead>IP</TableHead>
                    <TableHead className="text-right">Detalle</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {auditoriasFiltradas.map((auditoria) => {
                    const usuario = usuariosById.get(auditoria.usuarioId)
                    return (
                      <TableRow key={auditoria.id} className="border-border">
                        <TableCell className="whitespace-nowrap">{formatDateTime(auditoria.fecha)}</TableCell>
                        <TableCell>
                          <div className="font-medium">{usuario?.nombre || `Usuario ${auditoria.usuarioId}`}</div>
                          <div className="text-xs text-muted-foreground">{usuario?.correo || `ID ${auditoria.usuarioId}`}</div>
                        </TableCell>
                        <TableCell className="font-medium">{auditoria.entidad}</TableCell>
                        <TableCell>{auditoria.entidadId}</TableCell>
                        <TableCell>
                          <Badge className={getActionClass(auditoria.tipoAccion)}>
                            {auditoria.tipoAccion}
                          </Badge>
                        </TableCell>
                        <TableCell>{auditoria.ipOrigen}</TableCell>
                        <TableCell className="text-right">
                          <Button variant="outline" size="sm" onClick={() => setSelectedAudit(auditoria)}>
                            <Eye className="mr-2 h-4 w-4" />
                            Ver
                          </Button>
                        </TableCell>
                      </TableRow>
                    )
                  })}
                  {!isLoading && auditoriasFiltradas.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={7} className="py-8 text-center text-muted-foreground">
                        No se encontraron auditorias
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </div>

      <Dialog open={!!selectedAudit} onOpenChange={(open) => !open && setSelectedAudit(null)}>
        <DialogContent className="max-h-[90vh] max-w-3xl overflow-y-auto bg-card border-border">
          <DialogHeader>
            <DialogTitle>Detalle de auditoria</DialogTitle>
          </DialogHeader>
          {selectedAudit && (
            <div className="space-y-4">
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div className="rounded-lg border border-border p-3">
                  <p className="text-xs text-muted-foreground">Entidad</p>
                  <p className="font-medium">{selectedAudit.entidad} #{selectedAudit.entidadId}</p>
                </div>
                <div className="rounded-lg border border-border p-3">
                  <p className="text-xs text-muted-foreground">Accion</p>
                  <p className="font-medium">{selectedAudit.tipoAccion}</p>
                </div>
                <div className="rounded-lg border border-border p-3">
                  <p className="text-xs text-muted-foreground">Usuario</p>
                  <p className="font-medium">
                    {usuariosById.get(selectedAudit.usuarioId)?.nombre || `Usuario ${selectedAudit.usuarioId}`}
                  </p>
                </div>
                <div className="rounded-lg border border-border p-3">
                  <p className="text-xs text-muted-foreground">Fecha</p>
                  <p className="font-medium">{formatDateTime(selectedAudit.fecha)}</p>
                </div>
              </div>

              <div className="space-y-2">
                <p className="text-sm font-semibold">Datos anteriores</p>
                <pre className="max-h-72 overflow-auto rounded-lg border border-border bg-secondary p-3 text-xs">
                  {prettyJson(selectedAudit.datosAnteriores)}
                </pre>
              </div>

              <div className="space-y-2">
                <p className="text-sm font-semibold">Datos nuevos</p>
                <pre className="max-h-72 overflow-auto rounded-lg border border-border bg-secondary p-3 text-xs">
                  {prettyJson(selectedAudit.datosNuevos)}
                </pre>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
