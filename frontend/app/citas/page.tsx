"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
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
import { Calendar, Clock, Loader2, Plus, Search } from "lucide-react"
import { citaService, clienteService, ordenService, vehiculoService } from "@/lib/api"
import { useAuth } from "@/contexts/auth-context"
import type { Cita, CitaCreate, Cliente, EstadoOrden, Marca, Modelo, TipoServicio, Vehiculo } from "@/lib/api/types"

export default function CitasPage() {
  const { user } = useAuth()
  const [citas, setCitas] = useState<Cita[]>([])
  const [vehiculos, setVehiculos] = useState<Vehiculo[]>([])
  const [tiposServicio, setTiposServicio] = useState<TipoServicio[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")
  const [formData, setFormData] = useState<CitaCreate>({
    vehiculoId: 0,
    recepcionistaId: user?.id,
    tipoServicioId: 0,
    fechaCita: new Date().toISOString().split("T")[0],
    horaInicio: "08:00",
    horaFin: "09:00",
    estado: "Programada",
    observaciones: "",
  })

  const enrichCita = (cita: Cita, vehiculosData = vehiculos): Cita => ({
    ...cita,
    vehiculo: vehiculosData.find((vehiculo) => vehiculo.id === cita.vehiculoId),
  })

  const loadData = async () => {
    setIsLoading(true)
    try {
      const [clientesResponse, marcasResponse, modelosResponse, vehiculosResponse, tiposResponse, citasResponse] =
        await Promise.allSettled([
          clienteService.getAll({ pageNumber: 1, pageSize: 100 }),
          vehiculoService.getMarcas(),
          vehiculoService.getModelos(),
          vehiculoService.getAll({ pageNumber: 1, pageSize: 100 }),
          ordenService.getTiposServicio(),
          citaService.getAll({ pageNumber: 1, pageSize: 100 }),
        ])

      const clientesData: Cliente[] = clientesResponse.status === "fulfilled" ? clientesResponse.value.data : []
      const marcasData: Marca[] = marcasResponse.status === "fulfilled" ? marcasResponse.value : []
      const modelosData: Modelo[] = modelosResponse.status === "fulfilled" ? modelosResponse.value : []
      const vehiculosBase: Vehiculo[] = vehiculosResponse.status === "fulfilled" ? vehiculosResponse.value.data : []
      const tiposData: TipoServicio[] = tiposResponse.status === "fulfilled" ? tiposResponse.value : []
      const citasData: Cita[] = citasResponse.status === "fulfilled" ? citasResponse.value.data : []

      const vehiculosData = vehiculosBase.map((vehiculo) => {
        const modelo = modelosData.find((item) => item.id === vehiculo.modeloId)
        const marca = marcasData.find((item) => item.id === modelo?.marcaId)
        return {
          ...vehiculo,
          cliente: clientesData.find((cliente) => cliente.id === vehiculo.clienteId),
          modelo,
          marca,
          marcaId: modelo?.marcaId || vehiculo.marcaId || 0,
        }
      })

      setVehiculos(vehiculosData)
      setTiposServicio(tiposData)
      setCitas(citasData.map((cita) => enrichCita(cita, vehiculosData)))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const filteredCitas = citas.filter((cita) => {
    const text = searchTerm.toLowerCase()
    return (
      (cita.estado || "").toLowerCase().includes(text) ||
      (cita.observaciones || "").toLowerCase().includes(text) ||
      (cita.vehiculo?.placa || "").toLowerCase().includes(text) ||
      (cita.vehiculo?.cliente?.nombre || "").toLowerCase().includes(text)
    )
  })

  const handleCreate = async () => {
    setIsLoading(true)
    setErrorMessage("")
    try {
      const cita = await citaService.create({
        ...formData,
        recepcionistaId: user?.id,
      })
      setCitas([enrichCita(cita), ...citas])
      setIsCreateOpen(false)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No se pudo crear la cita.")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AppLayout>
      <Header title="Citas" subtitle="Agenda de servicios programados" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Calendar className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total citas</p>
                <p className="text-xl font-bold">{citas.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <Clock className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Programadas</p>
                <p className="text-xl font-bold">
                  {citas.filter((cita) => (cita.estado || "").toLowerCase().includes("program")).length}
                </p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-muted">
                <Calendar className="w-5 h-5 text-muted-foreground" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Vehiculos en agenda</p>
                <p className="text-xl font-bold">{new Set(citas.map((cita) => cita.vehiculoId)).size}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="flex flex-col sm:flex-row gap-4 justify-between">
          <div className="relative w-full sm:w-80">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por placa, cliente o estado..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="pl-9 bg-secondary border-border"
            />
          </div>
          <Button
            onClick={() => {
              setErrorMessage("")
              setIsCreateOpen(true)
            }}
            className="bg-primary text-primary-foreground"
          >
            <Plus className="w-4 h-4 mr-2" />
            Nueva Cita
          </Button>
        </div>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead>Fecha</TableHead>
                  <TableHead>Horario</TableHead>
                  <TableHead>Vehiculo</TableHead>
                  <TableHead>Tipo servicio</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead>Observaciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredCitas.map((cita) => (
                  <TableRow key={cita.id} className="border-border">
                    <TableCell>{cita.fechaCita}</TableCell>
                    <TableCell>{cita.horaInicio} - {cita.horaFin}</TableCell>
                    <TableCell>
                      <div className="font-medium">{cita.vehiculo?.placa || `Vehiculo ${cita.vehiculoId}`}</div>
                      <div className="text-sm text-muted-foreground">
                        {cita.vehiculo?.cliente?.nombre} {cita.vehiculo?.cliente?.apellido}
                      </div>
                    </TableCell>
                    <TableCell>
                      {tiposServicio.find((tipo) => tipo.id === cita.tipoServicioId)?.nombre || cita.tipoServicioId}
                    </TableCell>
                    <TableCell>
                      <Badge className="bg-primary/20 text-primary">{cita.estado}</Badge>
                    </TableCell>
                    <TableCell className="max-w-[280px] truncate">{cita.observaciones || "Sin observaciones"}</TableCell>
                  </TableRow>
                ))}
                {!isLoading && filteredCitas.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                      No se encontraron citas
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
            <DialogTitle>Nueva Cita</DialogTitle>
          </DialogHeader>
          {errorMessage && (
            <Alert variant="destructive">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Vehiculo</label>
              <Select value={formData.vehiculoId?.toString()} onValueChange={(value) => setFormData({ ...formData, vehiculoId: Number(value) })}>
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar vehiculo" />
                </SelectTrigger>
                <SelectContent>
                  {vehiculos.map((vehiculo) => (
                    <SelectItem key={vehiculo.id} value={vehiculo.id.toString()}>
                      {vehiculo.placa} - {vehiculo.cliente?.nombre} {vehiculo.cliente?.apellido}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Tipo de servicio</label>
              <Select value={formData.tipoServicioId?.toString()} onValueChange={(value) => setFormData({ ...formData, tipoServicioId: Number(value) })}>
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar servicio" />
                </SelectTrigger>
                <SelectContent>
                  {tiposServicio.map((tipo) => (
                    <SelectItem key={tipo.id} value={tipo.id.toString()}>
                      {tipo.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-3 gap-4">
              <Input type="date" value={formData.fechaCita} onChange={(event) => setFormData({ ...formData, fechaCita: event.target.value })} className="bg-secondary border-border" />
              <Input type="time" value={formData.horaInicio} onChange={(event) => setFormData({ ...formData, horaInicio: event.target.value })} className="bg-secondary border-border" />
              <Input type="time" value={formData.horaFin} onChange={(event) => setFormData({ ...formData, horaFin: event.target.value })} className="bg-secondary border-border" />
            </div>
            <Textarea
              value={formData.observaciones}
              onChange={(event) => setFormData({ ...formData, observaciones: event.target.value })}
              placeholder="Observaciones de la cita"
              className="bg-secondary border-border"
            />
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCreateOpen(false)}>Cancelar</Button>
            <Button disabled={isLoading || !formData.vehiculoId || !formData.tipoServicioId} onClick={handleCreate}>
              {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
