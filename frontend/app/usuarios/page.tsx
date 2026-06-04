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
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
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
import { KeyRound, Loader2, MoreHorizontal, Pencil, Plus, Search, Shield, UserCog, Users } from "lucide-react"
import { getApiErrorMessage, usuarioService } from "@/lib/api/services/usuario.service"
import type { Rol, RolCreate, Usuario, UsuarioCreate, UsuarioUpdate } from "@/lib/api/types"

const formatDate = (value?: string) => {
  if (!value) return "-"
  return new Date(value).toLocaleDateString("es-CO", {
    year: "numeric",
    month: "short",
    day: "2-digit",
  })
}

const initialUsuarioForm: UsuarioCreate = {
  rolId: 0,
  correo: "",
  nombre: "",
  contrasena: "",
}

const initialRolForm: RolCreate = {
  nombre: "",
  descripcion: "",
}

export default function UsuariosPage() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([])
  const [roles, setRoles] = useState<Rol[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState("")
  const [errorMessage, setErrorMessage] = useState("")
  const [isCreateUserOpen, setIsCreateUserOpen] = useState(false)
  const [isEditUserOpen, setIsEditUserOpen] = useState(false)
  const [isPasswordOpen, setIsPasswordOpen] = useState(false)
  const [isCreateRolOpen, setIsCreateRolOpen] = useState(false)
  const [selectedUser, setSelectedUser] = useState<Usuario | null>(null)
  const [usuarioForm, setUsuarioForm] = useState<UsuarioCreate>(initialUsuarioForm)
  const [editForm, setEditForm] = useState<UsuarioUpdate>({
    rolId: 0,
    correo: "",
    nombre: "",
  })
  const [passwordForm, setPasswordForm] = useState("")
  const [rolForm, setRolForm] = useState<RolCreate>(initialRolForm)

  const loadData = async () => {
    setIsLoading(true)
    setErrorMessage("")
    try {
      const [usuariosData, rolesData] = await Promise.all([
        usuarioService.getAll(),
        usuarioService.getRoles(),
      ])
      setUsuarios(usuariosData)
      setRoles(rolesData)
    } catch (error) {
      setErrorMessage(getApiErrorMessage(error, "No se pudieron cargar usuarios y roles."))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const rolesById = useMemo(() => new Map(roles.map((rol) => [rol.id, rol])), [roles])
  const usuariosActivos = usuarios.filter((usuario) => usuario.activo !== false)

  const filteredUsuarios = usuarios.filter((usuario) => {
    const text = searchTerm.toLowerCase()
    const rol = rolesById.get(usuario.rolId || 0)
    return (
      usuario.nombre.toLowerCase().includes(text) ||
      usuario.correo.toLowerCase().includes(text) ||
      (rol?.nombre || "").toLowerCase().includes(text)
    )
  })

  const resetMessages = () => {
    setMessage("")
    setErrorMessage("")
  }

  const handleOpenCreateUser = () => {
    resetMessages()
    setUsuarioForm({
      ...initialUsuarioForm,
      rolId: roles[0]?.id || 0,
    })
    setIsCreateUserOpen(true)
  }

  const handleCreateUser = async () => {
    resetMessages()
    setIsLoading(true)
    try {
      const usuario = await usuarioService.create(usuarioForm)
      setUsuarios([usuario, ...usuarios])
      setMessage("Usuario creado correctamente.")
      setIsCreateUserOpen(false)
    } catch (error) {
      setErrorMessage(getApiErrorMessage(error, "No se pudo crear el usuario."))
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenEditUser = (usuario: Usuario) => {
    resetMessages()
    setSelectedUser(usuario)
    setEditForm({
      rolId: usuario.rolId || roles[0]?.id || 0,
      correo: usuario.correo,
      nombre: usuario.nombre,
    })
    setIsEditUserOpen(true)
  }

  const handleUpdateUser = async () => {
    if (!selectedUser) return

    resetMessages()
    setIsLoading(true)
    try {
      await usuarioService.update(selectedUser.id, editForm)
      setUsuarios((current) =>
        current.map((usuario) =>
          usuario.id === selectedUser.id ? { ...usuario, ...editForm } : usuario
        )
      )
      setMessage("Usuario actualizado correctamente.")
      setIsEditUserOpen(false)
    } catch (error) {
      setErrorMessage(getApiErrorMessage(error, "No se pudo actualizar el usuario."))
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenPassword = (usuario: Usuario) => {
    resetMessages()
    setSelectedUser(usuario)
    setPasswordForm("")
    setIsPasswordOpen(true)
  }

  const handleResetPassword = async () => {
    if (!selectedUser) return

    resetMessages()
    setIsLoading(true)
    try {
      await usuarioService.resetPassword(selectedUser.id, { nuevaContrasena: passwordForm })
      setMessage("Contrasena actualizada correctamente.")
      setIsPasswordOpen(false)
    } catch (error) {
      setErrorMessage(getApiErrorMessage(error, "No se pudo actualizar la contrasena."))
    } finally {
      setIsLoading(false)
    }
  }

  const handleOpenCreateRol = () => {
    resetMessages()
    setRolForm(initialRolForm)
    setIsCreateRolOpen(true)
  }

  const handleCreateRol = async () => {
    resetMessages()
    setIsLoading(true)
    try {
      const rol = await usuarioService.createRol(rolForm)
      setRoles([rol, ...roles])
      setMessage("Rol creado correctamente.")
      setIsCreateRolOpen(false)
    } catch (error) {
      setErrorMessage(getApiErrorMessage(error, "No se pudo crear el rol."))
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AppLayout>
      <Header title="Usuarios y roles" subtitle="Administracion de accesos del sistema" />

      <div className="p-6 space-y-6">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
                <Users className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Usuarios</p>
                <p className="text-xl font-bold">{usuarios.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-success/10">
                <UserCog className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Activos</p>
                <p className="text-xl font-bold">{usuariosActivos.length}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-card border-border">
            <CardContent className="p-4 flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-warning/10">
                <Shield className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Roles</p>
                <p className="text-xl font-bold">{roles.length}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        {errorMessage && (
          <Alert variant="destructive">
            <AlertDescription>{errorMessage}</AlertDescription>
          </Alert>
        )}
        {message && (
          <Alert>
            <AlertDescription>{message}</AlertDescription>
          </Alert>
        )}

        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="relative w-full sm:w-96">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Buscar por nombre, correo o rol..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="pl-9 bg-secondary border-border"
            />
          </div>
          <div className="flex flex-col gap-2 sm:flex-row">
            <Button variant="outline" onClick={handleOpenCreateRol}>
              <Shield className="mr-2 h-4 w-4" />
              Nuevo Rol
            </Button>
            <Button onClick={handleOpenCreateUser}>
              <Plus className="mr-2 h-4 w-4" />
              Nuevo Usuario
            </Button>
          </div>
        </div>

        <Card className="bg-card border-border">
          <CardContent className="p-0">
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow className="border-border hover:bg-transparent">
                    <TableHead>Usuario</TableHead>
                    <TableHead>Correo</TableHead>
                    <TableHead>Rol</TableHead>
                    <TableHead>Estado</TableHead>
                    <TableHead>Creacion</TableHead>
                    <TableHead className="text-right">Acciones</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredUsuarios.map((usuario) => {
                    const rol = rolesById.get(usuario.rolId || 0)
                    return (
                      <TableRow key={usuario.id} className="border-border">
                        <TableCell className="font-medium">{usuario.nombre}</TableCell>
                        <TableCell>{usuario.correo}</TableCell>
                        <TableCell>
                          <Badge className="bg-primary/20 text-primary">
                            {rol?.nombre || usuario.rolId || "Sin rol"}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <Badge className={usuario.activo === false ? "bg-destructive/20 text-destructive" : "bg-success/20 text-success"}>
                            {usuario.activo === false ? "Inactivo" : "Activo"}
                          </Badge>
                        </TableCell>
                        <TableCell>{formatDate(usuario.fechaCreacion)}</TableCell>
                        <TableCell className="text-right">
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="ghost" size="icon">
                                <MoreHorizontal className="h-4 w-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem onClick={() => handleOpenEditUser(usuario)}>
                                <Pencil className="mr-2 h-4 w-4" />
                                Editar
                              </DropdownMenuItem>
                              <DropdownMenuItem onClick={() => handleOpenPassword(usuario)}>
                                <KeyRound className="mr-2 h-4 w-4" />
                                Cambiar contrasena
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </TableCell>
                      </TableRow>
                    )
                  })}
                  {!isLoading && filteredUsuarios.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={6} className="py-8 text-center text-muted-foreground">
                        No se encontraron usuarios
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-card border-border">
          <CardContent className="p-4">
            <h2 className="mb-3 text-sm font-semibold">Roles registrados</h2>
            <div className="grid grid-cols-1 gap-3 md:grid-cols-3">
              {roles.map((rol) => (
                <div key={rol.id} className="rounded-lg border border-border p-3">
                  <p className="font-medium">{rol.nombre}</p>
                  <p className="mt-1 text-sm text-muted-foreground">{rol.descripcion || "Sin descripcion"}</p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      <Dialog open={isCreateUserOpen} onOpenChange={setIsCreateUserOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Nuevo Usuario</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Rol</label>
              <Select
                value={usuarioForm.rolId.toString()}
                onValueChange={(value) => setUsuarioForm({ ...usuarioForm, rolId: Number(value) })}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar rol" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((rol) => (
                    <SelectItem key={rol.id} value={rol.id.toString()}>
                      {rol.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Nombre</label>
              <Input
                value={usuarioForm.nombre}
                onChange={(event) => setUsuarioForm({ ...usuarioForm, nombre: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Correo</label>
              <Input
                type="email"
                value={usuarioForm.correo}
                onChange={(event) => setUsuarioForm({ ...usuarioForm, correo: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Contrasena</label>
              <Input
                type="password"
                value={usuarioForm.contrasena}
                onChange={(event) => setUsuarioForm({ ...usuarioForm, contrasena: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCreateUserOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleCreateUser}
              disabled={isLoading || !usuarioForm.rolId || !usuarioForm.nombre || !usuarioForm.correo || !usuarioForm.contrasena}
            >
              {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isEditUserOpen} onOpenChange={setIsEditUserOpen}>
        <DialogContent className="bg-card border-border max-w-lg">
          <DialogHeader>
            <DialogTitle>Editar Usuario</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Rol</label>
              <Select
                value={editForm.rolId.toString()}
                onValueChange={(value) => setEditForm({ ...editForm, rolId: Number(value) })}
              >
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue placeholder="Seleccionar rol" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((rol) => (
                    <SelectItem key={rol.id} value={rol.id.toString()}>
                      {rol.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Nombre</label>
              <Input
                value={editForm.nombre}
                onChange={(event) => setEditForm({ ...editForm, nombre: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Correo</label>
              <Input
                type="email"
                value={editForm.correo}
                onChange={(event) => setEditForm({ ...editForm, correo: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsEditUserOpen(false)}>
              Cancelar
            </Button>
            <Button onClick={handleUpdateUser} disabled={isLoading || !editForm.rolId || !editForm.nombre || !editForm.correo}>
              {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Guardar cambios
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isPasswordOpen} onOpenChange={setIsPasswordOpen}>
        <DialogContent className="bg-card border-border max-w-md">
          <DialogHeader>
            <DialogTitle>Cambiar contrasena</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="rounded-lg border border-border bg-secondary/40 p-3 text-sm">
              {selectedUser?.nombre}
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Nueva contrasena</label>
              <Input
                type="password"
                value={passwordForm}
                onChange={(event) => setPasswordForm(event.target.value)}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsPasswordOpen(false)}>
              Cancelar
            </Button>
            <Button onClick={handleResetPassword} disabled={isLoading || !passwordForm}>
              {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Actualizar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={isCreateRolOpen} onOpenChange={setIsCreateRolOpen}>
        <DialogContent className="bg-card border-border max-w-md">
          <DialogHeader>
            <DialogTitle>Nuevo Rol</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Nombre</label>
              <Input
                value={rolForm.nombre}
                onChange={(event) => setRolForm({ ...rolForm, nombre: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Descripcion</label>
              <Input
                value={rolForm.descripcion}
                onChange={(event) => setRolForm({ ...rolForm, descripcion: event.target.value })}
                className="bg-secondary border-border"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsCreateRolOpen(false)}>
              Cancelar
            </Button>
            <Button onClick={handleCreateRol} disabled={isLoading || !rolForm.nombre || !rolForm.descripcion}>
              {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Guardar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </AppLayout>
  )
}
