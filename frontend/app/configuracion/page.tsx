"use client"

import { useEffect, useState } from "react"
import { AppLayout, Header } from "@/components/layout"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Car, CheckCircle2, Loader2, Lock, MapPin, Plus, Settings, Shield } from "lucide-react"
import { useAuth } from "@/contexts/auth-context"
import { ubicacionService, vehiculoService } from "@/lib/api"
import type { Ciudad, Departamento, Marca, Modelo, Pais } from "@/lib/api/types"

const getRoleName = (rol: unknown) => {
  if (typeof rol === "string") return rol
  if (rol && typeof rol === "object" && "nombre" in rol) {
    return String((rol as { nombre?: string }).nombre || "")
  }
  return "Sin rol"
}

const roleModules: Record<string, string[]> = {
  admin: [
    "Usuarios y roles",
    "Clientes y vehiculos",
    "Citas y ordenes",
    "Inventario de repuestos",
    "Facturacion y pagos",
    "Catalogos de configuracion",
    "Auditoria",
  ],
  recepcionista: [
    "Clientes y vehiculos",
    "Citas",
    "Ordenes de servicio",
    "Consulta de catalogos operativos",
  ],
  mecanico: [
    "Ordenes asignadas",
    "Tareas mecanicas",
    "Detalle de repuestos usados",
    "Facturacion permitida por backend",
  ],
}

const roleRestrictions: Record<string, string[]> = {
  admin: ["No tiene restricciones funcionales principales."],
  recepcionista: ["No administra usuarios.", "No modifica inventario critico.", "No accede a configuracion global."],
  mecanico: ["No crea clientes.", "No elimina repuestos.", "No administra usuarios ni roles."],
}

export default function ConfiguracionPage() {
  const { user } = useAuth()
  const roleName = getRoleName(user?.rol)
  const normalizedRole = roleName.toLowerCase()
  const modules = roleModules[normalizedRole] || ["Accesos definidos por politicas del backend."]
  const restrictions = roleRestrictions[normalizedRole] || ["Restricciones definidas por politicas del backend."]
  const isAdmin = normalizedRole === "admin"
  const [marcas, setMarcas] = useState<Marca[]>([])
  const [modelos, setModelos] = useState<Modelo[]>([])
  const [marcaNombre, setMarcaNombre] = useState("")
  const [modeloForm, setModeloForm] = useState({
    marcaId: 0,
    nombre: "",
    anioDesde: new Date().getFullYear(),
    anioHasta: new Date().getFullYear() + 1,
  })
  const [isLoadingCatalogos, setIsLoadingCatalogos] = useState(false)
  const [catalogoMessage, setCatalogoMessage] = useState("")
  const [catalogoError, setCatalogoError] = useState("")
  const [paises, setPaises] = useState<Pais[]>([])
  const [departamentos, setDepartamentos] = useState<Departamento[]>([])
  const [ciudades, setCiudades] = useState<Ciudad[]>([])
  const [paisForm, setPaisForm] = useState({ nombre: "", codigo: "" })
  const [departamentoForm, setDepartamentoForm] = useState({ paisId: 0, nombre: "" })
  const [ciudadPaisId, setCiudadPaisId] = useState(0)
  const [ciudadForm, setCiudadForm] = useState({ departamentoId: 0, nombre: "" })
  const [isLoadingUbicaciones, setIsLoadingUbicaciones] = useState(false)
  const [ubicacionMessage, setUbicacionMessage] = useState("")
  const [ubicacionError, setUbicacionError] = useState("")

  const loadCatalogosVehiculo = async () => {
    if (!isAdmin) return
    setIsLoadingCatalogos(true)
    try {
      const [marcasData, modelosData] = await Promise.all([
        vehiculoService.getMarcas(),
        vehiculoService.getModelos(),
      ])

      setMarcas(marcasData)
      setModelos(modelosData)
      setModeloForm((current) => ({
        ...current,
        marcaId: current.marcaId || marcasData[0]?.id || 0,
      }))
    } finally {
      setIsLoadingCatalogos(false)
    }
  }

  const loadCatalogosUbicacion = async () => {
    if (!isAdmin) return
    setIsLoadingUbicaciones(true)
    try {
      const [paisesData, departamentosData, ciudadesData] = await Promise.all([
        ubicacionService.getPaises(),
        ubicacionService.getDepartamentos(),
        ubicacionService.getCiudades(),
      ])

      setPaises(paisesData)
      setDepartamentos(departamentosData)
      setCiudades(ciudadesData)

      setDepartamentoForm((current) => ({
        ...current,
        paisId: current.paisId || paisesData[0]?.id || 0,
      }))

      const nextPaisId = ciudadPaisId || paisesData[0]?.id || 0
      const availableDepartments = departamentosData.filter((item) => item.paisId === nextPaisId)
      setCiudadPaisId(nextPaisId)
      setCiudadForm((current) => ({
        ...current,
        departamentoId:
          current.departamentoId && availableDepartments.some((item) => item.id === current.departamentoId)
            ? current.departamentoId
            : availableDepartments[0]?.id || departamentosData[0]?.id || 0,
      }))
    } finally {
      setIsLoadingUbicaciones(false)
    }
  }

  useEffect(() => {
    loadCatalogosVehiculo()
    loadCatalogosUbicacion()
  }, [isAdmin])

  const handleCreateMarca = async () => {
    setCatalogoError("")
    setCatalogoMessage("")
    setIsLoadingCatalogos(true)
    try {
      const marca = await vehiculoService.createMarca(marcaNombre.trim())
      setMarcaNombre("")
      setCatalogoMessage(`Marca ${marca.nombre} creada correctamente.`)
      await loadCatalogosVehiculo()
    } catch (error) {
      setCatalogoError(error instanceof Error ? error.message : "No se pudo crear la marca.")
    } finally {
      setIsLoadingCatalogos(false)
    }
  }

  const handleCreateModelo = async () => {
    setCatalogoError("")
    setCatalogoMessage("")
    setIsLoadingCatalogos(true)
    try {
      const modelo = await vehiculoService.createModelo(modeloForm)
      setModeloForm((current) => ({ ...current, nombre: "" }))
      setCatalogoMessage(`Modelo ${modelo.nombre} creado correctamente.`)
      await loadCatalogosVehiculo()
    } catch (error) {
      setCatalogoError(error instanceof Error ? error.message : "No se pudo crear el modelo.")
    } finally {
      setIsLoadingCatalogos(false)
    }
  }

  const handleCreatePais = async () => {
    setUbicacionError("")
    setUbicacionMessage("")
    setIsLoadingUbicaciones(true)
    try {
      const pais = await ubicacionService.createPais({
        nombre: paisForm.nombre.trim(),
        codigo: paisForm.codigo.trim().toUpperCase(),
      })
      setPaisForm({ nombre: "", codigo: "" })
      setUbicacionMessage(`Pais ${pais.nombre} creado correctamente.`)
      await loadCatalogosUbicacion()
    } catch (error) {
      setUbicacionError(error instanceof Error ? error.message : "No se pudo crear el pais.")
    } finally {
      setIsLoadingUbicaciones(false)
    }
  }

  const handleCreateDepartamento = async () => {
    setUbicacionError("")
    setUbicacionMessage("")
    setIsLoadingUbicaciones(true)
    try {
      const departamento = await ubicacionService.createDepartamento({
        paisId: departamentoForm.paisId,
        nombre: departamentoForm.nombre.trim(),
      })
      setDepartamentoForm((current) => ({ ...current, nombre: "" }))
      setCiudadPaisId(departamento.paisId)
      setCiudadForm((current) => ({ ...current, departamentoId: departamento.id }))
      setUbicacionMessage(`Departamento ${departamento.nombre} creado correctamente.`)
      await loadCatalogosUbicacion()
    } catch (error) {
      setUbicacionError(error instanceof Error ? error.message : "No se pudo crear el departamento.")
    } finally {
      setIsLoadingUbicaciones(false)
    }
  }

  const handleCreateCiudad = async () => {
    setUbicacionError("")
    setUbicacionMessage("")
    setIsLoadingUbicaciones(true)
    try {
      const ciudad = await ubicacionService.createCiudad({
        departamentoId: ciudadForm.departamentoId,
        nombre: ciudadForm.nombre.trim(),
      })
      setCiudadForm((current) => ({ ...current, nombre: "" }))
      setUbicacionMessage(`Ciudad ${ciudad.nombre} creada correctamente.`)
      await loadCatalogosUbicacion()
    } catch (error) {
      setUbicacionError(error instanceof Error ? error.message : "No se pudo crear la ciudad.")
    } finally {
      setIsLoadingUbicaciones(false)
    }
  }

  const ciudadDepartamentos = departamentos.filter((departamento) => departamento.paisId === ciudadPaisId)

  return (
    <AppLayout>
      <Header title="Configuracion" subtitle="Preferencias visibles y permisos por rol" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                <Settings className="w-5 h-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Cuenta activa</p>
                <p className="text-lg font-semibold">{user?.nombre || "Usuario"}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-success/10">
                <Shield className="w-5 h-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Rol actual</p>
                <p className="text-lg font-semibold">{roleName}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-muted">
                <Lock className="w-5 h-5 text-muted-foreground" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Politicas</p>
                <p className="text-lg font-semibold">Backend JWT</p>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card className="bg-card border-border">
            <CardHeader>
              <CardTitle>Modulos disponibles</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {modules.map((module) => (
                <div key={module} className="flex items-center justify-between rounded-lg border border-border p-3">
                  <div className="flex items-center gap-3">
                    <CheckCircle2 className="w-4 h-4 text-success" />
                    <span className="text-sm">{module}</span>
                  </div>
                  <Badge className="bg-success/20 text-success">Permitido</Badge>
                </div>
              ))}
            </CardContent>
          </Card>

          <Card className="bg-card border-border">
            <CardHeader>
              <CardTitle>Restricciones del rol</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {restrictions.map((restriction) => (
                <div key={restriction} className="flex items-center justify-between rounded-lg border border-border p-3">
                  <div className="flex items-center gap-3">
                    <Lock className="w-4 h-4 text-muted-foreground" />
                    <span className="text-sm">{restriction}</span>
                  </div>
                  <Badge variant="secondary">Controlado</Badge>
                </div>
              ))}
            </CardContent>
          </Card>
        </div>

        {isAdmin && (
          <Card className="bg-card border-border">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <MapPin className="w-5 h-5 text-primary" />
                Catalogos de ubicacion
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-5">
              {ubicacionError && (
                <Alert variant="destructive">
                  <AlertDescription>{ubicacionError}</AlertDescription>
                </Alert>
              )}
              {ubicacionMessage && (
                <Alert>
                  <AlertDescription>{ubicacionMessage}</AlertDescription>
                </Alert>
              )}

              <div className="grid grid-cols-1 xl:grid-cols-3 gap-5">
                <div className="rounded-lg border border-border p-4 space-y-4">
                  <div>
                    <p className="font-medium">Nuevo pais</p>
                    <p className="text-sm text-muted-foreground">
                      Crea el pais base para departamentos y ciudades.
                    </p>
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-[1fr_110px] gap-3">
                    <Input
                      value={paisForm.nombre}
                      onChange={(event) => setPaisForm({ ...paisForm, nombre: event.target.value })}
                      placeholder="Ej: Colombia"
                      className="bg-secondary border-border"
                    />
                    <Input
                      value={paisForm.codigo}
                      onChange={(event) =>
                        setPaisForm({ ...paisForm, codigo: event.target.value.toUpperCase() })
                      }
                      placeholder="CO"
                      maxLength={5}
                      className="bg-secondary border-border uppercase"
                    />
                  </div>
                  <Button
                    onClick={handleCreatePais}
                    disabled={isLoadingUbicaciones || !paisForm.nombre.trim() || !paisForm.codigo.trim()}
                    className="bg-primary text-primary-foreground"
                  >
                    {isLoadingUbicaciones ? (
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    ) : (
                      <Plus className="w-4 h-4 mr-2" />
                    )}
                    Crear pais
                  </Button>
                </div>

                <div className="rounded-lg border border-border p-4 space-y-4">
                  <div>
                    <p className="font-medium">Nuevo departamento</p>
                    <p className="text-sm text-muted-foreground">
                      Asocia el departamento a un pais existente.
                    </p>
                  </div>
                  <Select
                    value={departamentoForm.paisId.toString()}
                    onValueChange={(value) =>
                      setDepartamentoForm({ ...departamentoForm, paisId: parseInt(value) })
                    }
                  >
                    <SelectTrigger className="bg-secondary border-border">
                      <SelectValue placeholder="Seleccionar pais" />
                    </SelectTrigger>
                    <SelectContent>
                      {paises.map((pais) => (
                        <SelectItem key={pais.id} value={pais.id.toString()}>
                          {pais.nombre}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <Input
                    value={departamentoForm.nombre}
                    onChange={(event) =>
                      setDepartamentoForm({ ...departamentoForm, nombre: event.target.value })
                    }
                    placeholder="Ej: Antioquia"
                    className="bg-secondary border-border"
                  />
                  <Button
                    onClick={handleCreateDepartamento}
                    disabled={
                      isLoadingUbicaciones || !departamentoForm.paisId || !departamentoForm.nombre.trim()
                    }
                    className="bg-primary text-primary-foreground"
                  >
                    {isLoadingUbicaciones ? (
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    ) : (
                      <Plus className="w-4 h-4 mr-2" />
                    )}
                    Crear departamento
                  </Button>
                </div>

                <div className="rounded-lg border border-border p-4 space-y-4">
                  <div>
                    <p className="font-medium">Nueva ciudad</p>
                    <p className="text-sm text-muted-foreground">
                      Selecciona pais y departamento antes de crearla.
                    </p>
                  </div>
                  <Select
                    value={ciudadPaisId.toString()}
                    onValueChange={(value) => {
                      const nextPaisId = parseInt(value)
                      const nextDepartments = departamentos.filter((item) => item.paisId === nextPaisId)
                      setCiudadPaisId(nextPaisId)
                      setCiudadForm({
                        ...ciudadForm,
                        departamentoId: nextDepartments[0]?.id || 0,
                      })
                    }}
                  >
                    <SelectTrigger className="bg-secondary border-border">
                      <SelectValue placeholder="Seleccionar pais" />
                    </SelectTrigger>
                    <SelectContent>
                      {paises.map((pais) => (
                        <SelectItem key={pais.id} value={pais.id.toString()}>
                          {pais.nombre}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <Select
                    value={ciudadForm.departamentoId.toString()}
                    onValueChange={(value) =>
                      setCiudadForm({ ...ciudadForm, departamentoId: parseInt(value) })
                    }
                  >
                    <SelectTrigger className="bg-secondary border-border">
                      <SelectValue placeholder="Seleccionar departamento" />
                    </SelectTrigger>
                    <SelectContent>
                      {ciudadDepartamentos.map((departamento) => (
                        <SelectItem key={departamento.id} value={departamento.id.toString()}>
                          {departamento.nombre}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <Input
                    value={ciudadForm.nombre}
                    onChange={(event) => setCiudadForm({ ...ciudadForm, nombre: event.target.value })}
                    placeholder="Ej: Medellin"
                    className="bg-secondary border-border"
                  />
                  <Button
                    onClick={handleCreateCiudad}
                    disabled={isLoadingUbicaciones || !ciudadForm.departamentoId || !ciudadForm.nombre.trim()}
                    className="bg-primary text-primary-foreground"
                  >
                    {isLoadingUbicaciones ? (
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    ) : (
                      <Plus className="w-4 h-4 mr-2" />
                    )}
                    Crear ciudad
                  </Button>
                </div>
              </div>

              <div className="grid grid-cols-1 xl:grid-cols-3 gap-5">
                <div className="rounded-lg border border-border p-4 space-y-3">
                  <p className="text-sm font-medium">Paises registrados</p>
                  <div className="flex flex-wrap gap-2">
                    {paises.map((pais) => (
                      <Badge key={pais.id} variant="secondary">
                        {pais.nombre} ({pais.codigo})
                      </Badge>
                    ))}
                    {paises.length === 0 && (
                      <p className="text-sm text-muted-foreground">No hay paises registrados.</p>
                    )}
                  </div>
                </div>

                <div className="rounded-lg border border-border overflow-hidden">
                  <div className="grid grid-cols-2 gap-3 px-4 py-3 text-sm font-medium bg-secondary/50">
                    <span>Pais</span>
                    <span>Departamento</span>
                  </div>
                  <div className="divide-y divide-border max-h-64 overflow-auto">
                    {departamentos.map((departamento) => {
                      const pais = paises.find((item) => item.id === departamento.paisId)
                      return (
                        <div key={departamento.id} className="grid grid-cols-2 gap-3 px-4 py-3 text-sm">
                          <span>{pais?.nombre || `Pais ${departamento.paisId}`}</span>
                          <span>{departamento.nombre}</span>
                        </div>
                      )
                    })}
                    {departamentos.length === 0 && (
                      <div className="px-4 py-6 text-sm text-muted-foreground text-center">
                        No hay departamentos registrados.
                      </div>
                    )}
                  </div>
                </div>

                <div className="rounded-lg border border-border overflow-hidden">
                  <div className="grid grid-cols-3 gap-3 px-4 py-3 text-sm font-medium bg-secondary/50">
                    <span>Pais</span>
                    <span>Departamento</span>
                    <span>Ciudad</span>
                  </div>
                  <div className="divide-y divide-border max-h-64 overflow-auto">
                    {ciudades.map((ciudad) => {
                      const departamento = departamentos.find((item) => item.id === ciudad.departamentoId)
                      const pais = paises.find((item) => item.id === departamento?.paisId)
                      return (
                        <div key={ciudad.id} className="grid grid-cols-3 gap-3 px-4 py-3 text-sm">
                          <span>{pais?.nombre || "-"}</span>
                          <span>{departamento?.nombre || `Departamento ${ciudad.departamentoId}`}</span>
                          <span>{ciudad.nombre}</span>
                        </div>
                      )
                    })}
                    {ciudades.length === 0 && (
                      <div className="px-4 py-6 text-sm text-muted-foreground text-center">
                        No hay ciudades registradas.
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {isAdmin && (
          <Card className="bg-card border-border">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Car className="w-5 h-5 text-primary" />
                Catalogos de vehiculos
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-5">
              {catalogoError && (
                <Alert variant="destructive">
                  <AlertDescription>{catalogoError}</AlertDescription>
                </Alert>
              )}
              {catalogoMessage && (
                <Alert>
                  <AlertDescription>{catalogoMessage}</AlertDescription>
                </Alert>
              )}

              <div className="grid grid-cols-1 xl:grid-cols-2 gap-5">
                <div className="rounded-lg border border-border p-4 space-y-4">
                  <div>
                    <p className="font-medium">Nueva marca</p>
                    <p className="text-sm text-muted-foreground">
                      Crea una marca para asociarle modelos.
                    </p>
                  </div>
                  <div className="flex flex-col sm:flex-row gap-3">
                    <Input
                      value={marcaNombre}
                      onChange={(event) => setMarcaNombre(event.target.value)}
                      placeholder="Ej: Toyota"
                      className="bg-secondary border-border"
                    />
                    <Button
                      onClick={handleCreateMarca}
                      disabled={isLoadingCatalogos || !marcaNombre.trim()}
                      className="bg-primary text-primary-foreground"
                    >
                      {isLoadingCatalogos ? (
                        <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                      ) : (
                        <Plus className="w-4 h-4 mr-2" />
                      )}
                      Crear marca
                    </Button>
                  </div>
                  <div className="space-y-2">
                    <p className="text-sm font-medium">Marcas disponibles</p>
                    <div className="flex flex-wrap gap-2">
                      {marcas.map((marca) => (
                        <Badge key={marca.id} variant="secondary">
                          {marca.nombre}
                        </Badge>
                      ))}
                    </div>
                  </div>
                </div>

                <div className="rounded-lg border border-border p-4 space-y-4">
                  <div>
                    <p className="font-medium">Nuevo modelo</p>
                    <p className="text-sm text-muted-foreground">
                      Crea un modelo dentro de una marca existente.
                    </p>
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    <div className="space-y-2">
                      <label className="text-sm font-medium">Marca</label>
                      <Select
                        value={modeloForm.marcaId.toString()}
                        onValueChange={(value) =>
                          setModeloForm({ ...modeloForm, marcaId: parseInt(value) })
                        }
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
                      <Input
                        value={modeloForm.nombre}
                        onChange={(event) =>
                          setModeloForm({ ...modeloForm, nombre: event.target.value })
                        }
                        placeholder="Ej: Hilux"
                        className="bg-secondary border-border"
                      />
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm font-medium">Anio desde</label>
                      <Input
                        type="number"
                        value={modeloForm.anioDesde}
                        onChange={(event) =>
                          setModeloForm({ ...modeloForm, anioDesde: Number(event.target.value) || 0 })
                        }
                        className="bg-secondary border-border"
                      />
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm font-medium">Anio hasta</label>
                      <Input
                        type="number"
                        value={modeloForm.anioHasta}
                        onChange={(event) =>
                          setModeloForm({ ...modeloForm, anioHasta: Number(event.target.value) || 0 })
                        }
                        className="bg-secondary border-border"
                      />
                    </div>
                  </div>
                  <Button
                    onClick={handleCreateModelo}
                    disabled={
                      isLoadingCatalogos ||
                      !modeloForm.marcaId ||
                      !modeloForm.nombre.trim() ||
                      modeloForm.anioDesde <= 0 ||
                      modeloForm.anioHasta < modeloForm.anioDesde
                    }
                    className="bg-primary text-primary-foreground"
                  >
                    {isLoadingCatalogos ? (
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    ) : (
                      <Plus className="w-4 h-4 mr-2" />
                    )}
                    Crear modelo
                  </Button>
                </div>
              </div>

              <div className="rounded-lg border border-border overflow-hidden">
                <div className="grid grid-cols-[1fr_1fr_120px_120px] gap-3 px-4 py-3 text-sm font-medium bg-secondary/50">
                  <span>Marca</span>
                  <span>Modelo</span>
                  <span>Desde</span>
                  <span>Hasta</span>
                </div>
                <div className="divide-y divide-border">
                  {modelos.map((modelo) => {
                    const marca = marcas.find((item) => item.id === modelo.marcaId)
                    return (
                      <div
                        key={modelo.id}
                        className="grid grid-cols-[1fr_1fr_120px_120px] gap-3 px-4 py-3 text-sm"
                      >
                        <span>{marca?.nombre || `Marca ${modelo.marcaId}`}</span>
                        <span>{modelo.nombre}</span>
                        <span>{modelo.anioDesde || "-"}</span>
                        <span>{modelo.anioHasta || "-"}</span>
                      </div>
                    )
                  })}
                  {modelos.length === 0 && (
                    <div className="px-4 py-6 text-sm text-muted-foreground text-center">
                      No hay modelos registrados.
                    </div>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        )}
      </div>
    </AppLayout>
  )
}
