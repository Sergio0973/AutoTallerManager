"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import { useState } from "react"
import {
  Calendar,
  Car,
  ChevronLeft,
  ChevronRight,
  ClipboardList,
  LayoutDashboard,
  LogOut,
  Package,
  Receipt,
  Settings,
  ShieldCheck,
  UserCog,
  Users,
  Wrench,
} from "lucide-react"
import { useAuth } from "@/contexts/auth-context"
import { cn } from "@/lib/utils"

const navigationItems = [
  {
    title: "Dashboard",
    href: "/dashboard",
    icon: LayoutDashboard,
    roles: ["Admin", "Recepcionista", "Mecanico"],
  },
  {
    title: "Clientes",
    href: "/clientes",
    icon: Users,
    roles: ["Admin", "Recepcionista"],
  },
  {
    title: "Usuarios",
    href: "/usuarios",
    icon: UserCog,
    roles: ["Admin"],
  },
  {
    title: "Vehiculos",
    href: "/vehiculos",
    icon: Car,
    roles: ["Admin", "Recepcionista"],
  },
  {
    title: "Citas",
    href: "/citas",
    icon: Calendar,
    roles: ["Admin", "Recepcionista"],
  },
  {
    title: "Ordenes de Servicio",
    href: "/ordenes",
    icon: ClipboardList,
    roles: ["Admin", "Recepcionista", "Mecanico"],
  },
  {
    title: "Inventario",
    href: "/inventario",
    icon: Package,
    roles: ["Admin"],
  },
  {
    title: "Facturacion",
    href: "/facturacion",
    icon: Receipt,
    roles: ["Admin", "Mecanico"],
  },
  {
    title: "Auditoria",
    href: "/auditoria",
    icon: ShieldCheck,
    roles: ["Admin"],
  },
]

const bottomItems = [
  {
    title: "Configuracion",
    href: "/configuracion",
    icon: Settings,
    roles: ["Admin", "Recepcionista", "Mecanico"],
  },
]

const getRoleName = (rol: unknown) => {
  if (typeof rol === "string") return rol
  if (rol && typeof rol === "object" && "nombre" in rol) {
    return String((rol as { nombre?: string }).nombre || "")
  }

  return ""
}

export function Sidebar() {
  const pathname = usePathname()
  const [collapsed, setCollapsed] = useState(false)
  const { logout, user } = useAuth()
  const roleName = getRoleName(user?.rol)
  const canAccess = (roles: string[]) =>
    roles.some((role) => role.toLowerCase() === roleName.toLowerCase())
  const visibleNavigationItems = navigationItems.filter((item) => canAccess(item.roles))
  const visibleBottomItems = bottomItems.filter((item) => canAccess(item.roles))

  return (
    <aside
      className={cn(
        "flex flex-col h-screen bg-sidebar border-r border-sidebar-border transition-all duration-300",
        collapsed ? "w-16" : "w-64"
      )}
    >
      <div className="flex items-center gap-3 px-4 h-16 border-b border-sidebar-border">
        <div className="flex items-center justify-center w-8 h-8 rounded-lg bg-primary">
          <Wrench className="w-5 h-5 text-primary-foreground" />
        </div>
        {!collapsed && (
          <div className="flex flex-col">
            <span className="text-sm font-semibold text-sidebar-foreground">
              AutoTaller
            </span>
            <span className="text-xs text-muted-foreground">Manager</span>
          </div>
        )}
      </div>

      <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
        <div className={cn("mb-2", !collapsed && "px-2")}>
          {!collapsed && (
            <span className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
              Principal
            </span>
          )}
        </div>
        {visibleNavigationItems.map((item) => {
          const isActive = pathname === item.href || pathname?.startsWith(`${item.href}/`)

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors",
                isActive
                  ? "bg-sidebar-accent text-sidebar-primary"
                  : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
              )}
            >
              <item.icon className={cn("w-5 h-5 flex-shrink-0", isActive && "text-sidebar-primary")} />
              {!collapsed && <span>{item.title}</span>}
            </Link>
          )
        })}
      </nav>

      <div className="px-2 py-4 border-t border-sidebar-border space-y-1">
        {visibleBottomItems.map((item) => {
          const isActive = pathname === item.href

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors",
                isActive
                  ? "bg-sidebar-accent text-sidebar-primary"
                  : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
              )}
            >
              <item.icon className="w-5 h-5 flex-shrink-0" />
              {!collapsed && <span>{item.title}</span>}
            </Link>
          )
        })}
        <button
          onClick={logout}
          className="flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium w-full text-sidebar-foreground hover:bg-destructive/10 hover:text-destructive transition-colors"
        >
          <LogOut className="w-5 h-5 flex-shrink-0" />
          {!collapsed && <span>Cerrar Sesion</span>}
        </button>
      </div>

      <button
        onClick={() => setCollapsed(!collapsed)}
        className="flex items-center justify-center h-10 border-t border-sidebar-border text-muted-foreground hover:text-foreground hover:bg-sidebar-accent transition-colors"
      >
        {collapsed ? (
          <ChevronRight className="w-4 h-4" />
        ) : (
          <ChevronLeft className="w-4 h-4" />
        )}
      </button>
    </aside>
  )
}
