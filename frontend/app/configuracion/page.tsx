"use client"

import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { CheckCircle2, Lock, Settings, Shield } from "lucide-react"
import { useAuth } from "@/contexts/auth-context"

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
      </div>
    </AppLayout>
  )
}
