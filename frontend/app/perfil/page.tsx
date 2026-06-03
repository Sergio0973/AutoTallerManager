"use client"

import { AppLayout, Header } from "@/components/layout"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Mail, Shield, User } from "lucide-react"
import { useAuth } from "@/contexts/auth-context"

const getRoleName = (rol: unknown) => {
  if (typeof rol === "string") return rol
  if (rol && typeof rol === "object" && "nombre" in rol) {
    return String((rol as { nombre?: string }).nombre || "")
  }
  return "Sin rol"
}

const roleDescriptions: Record<string, string> = {
  admin: "Acceso total al sistema, usuarios, catalogos, inventario, facturacion y configuracion.",
  recepcionista: "Gestion de clientes, vehiculos, citas y ordenes de servicio.",
  mecanico: "Gestion operativa de ordenes, tareas, repuestos usados, facturas y pagos permitidos.",
}

export default function PerfilPage() {
  const { user } = useAuth()
  const roleName = getRoleName(user?.rol)
  const description = roleDescriptions[roleName.toLowerCase()] || "Permisos definidos por el backend."

  return (
    <AppLayout>
      <Header title="Mi Perfil" subtitle="Informacion de la cuenta autenticada" />

      <div className="p-6 space-y-6">
        <Card className="bg-card border-border">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="w-5 h-5 text-primary" />
              Datos de usuario
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-5">
            <div className="flex items-center gap-4">
              <div className="flex items-center justify-center w-16 h-16 rounded-full bg-primary/20">
                <User className="w-8 h-8 text-primary" />
              </div>
              <div>
                <h2 className="text-xl font-semibold">{user?.nombre || "Usuario"}</h2>
                <p className="text-sm text-muted-foreground">{user?.correo || "Sin correo"}</p>
                <Badge className="mt-2 bg-primary/20 text-primary">{roleName}</Badge>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4 border-t border-border">
              <div className="space-y-2">
                <label className="text-sm font-medium flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  Nombre
                </label>
                <Input value={user?.nombre || ""} readOnly className="bg-secondary border-border" />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium flex items-center gap-2">
                  <Mail className="w-4 h-4 text-muted-foreground" />
                  Correo
                </label>
                <Input value={user?.correo || ""} readOnly className="bg-secondary border-border" />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium flex items-center gap-2">
                  <Shield className="w-4 h-4 text-muted-foreground" />
                  Rol
                </label>
                <Input value={roleName} readOnly className="bg-secondary border-border" />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Id de usuario</label>
                <Input value={user?.id?.toString() || ""} readOnly className="bg-secondary border-border" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-card border-border">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Shield className="w-5 h-5 text-primary" />
              Alcance del rol
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground">{description}</p>
          </CardContent>
        </Card>
      </div>
    </AppLayout>
  )
}
