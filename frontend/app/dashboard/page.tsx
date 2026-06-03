import { AppLayout, Header } from "@/components/layout"
import { 
  Users, 
  Car, 
  ClipboardList, 
  Package, 
  TrendingUp, 
  AlertTriangle,
  Clock,
  CheckCircle,
  DollarSign
} from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

const stats = [
  {
    title: "Clientes Activos",
    value: "248",
    change: "+12%",
    trend: "up",
    icon: Users,
  },
  {
    title: "Vehículos Registrados",
    value: "412",
    change: "+8%",
    trend: "up",
    icon: Car,
  },
  {
    title: "Órdenes en Proceso",
    value: "23",
    change: "-5%",
    trend: "down",
    icon: ClipboardList,
  },
  {
    title: "Repuestos Bajo Stock",
    value: "7",
    change: "+2",
    trend: "warning",
    icon: Package,
  },
]

const recentOrders = [
  {
    id: "ORD-001",
    cliente: "Carlos Mendoza",
    vehiculo: "Toyota Corolla 2020",
    estado: "En proceso",
    statusColor: "bg-primary/20 text-primary",
  },
  {
    id: "ORD-002",
    cliente: "María García",
    vehiculo: "Honda Civic 2019",
    estado: "Pendiente",
    statusColor: "bg-warning/20 text-warning",
  },
  {
    id: "ORD-003",
    cliente: "Juan Pérez",
    vehiculo: "Ford Focus 2021",
    estado: "Completada",
    statusColor: "bg-success/20 text-success",
  },
  {
    id: "ORD-004",
    cliente: "Ana Rodríguez",
    vehiculo: "Chevrolet Cruze 2018",
    estado: "En proceso",
    statusColor: "bg-primary/20 text-primary",
  },
]

const lowStockItems = [
  { name: "Filtro de Aceite Universal", stock: 3, minStock: 10 },
  { name: "Pastillas de Freno Delanteras", stock: 5, minStock: 15 },
  { name: "Bujías NGK", stock: 8, minStock: 20 },
]

export default function DashboardPage() {
  return (
    <AppLayout>
      <Header 
        title="Dashboard" 
        subtitle="Vista general del taller" 
      />
      
      <div className="p-6 space-y-6">
        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {stats.map((stat) => (
            <Card key={stat.title} className="bg-card border-border">
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-muted-foreground">{stat.title}</p>
                    <p className="text-2xl font-bold text-foreground mt-1">
                      {stat.value}
                    </p>
                    <p className={`text-xs mt-1 ${
                      stat.trend === "up" 
                        ? "text-success" 
                        : stat.trend === "warning" 
                        ? "text-warning" 
                        : "text-destructive"
                    }`}>
                      {stat.change} vs mes anterior
                    </p>
                  </div>
                  <div className={`flex items-center justify-center w-12 h-12 rounded-lg ${
                    stat.trend === "warning" 
                      ? "bg-warning/10" 
                      : "bg-primary/10"
                  }`}>
                    <stat.icon className={`w-6 h-6 ${
                      stat.trend === "warning" 
                        ? "text-warning" 
                        : "text-primary"
                    }`} />
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Recent Orders */}
          <Card className="lg:col-span-2 bg-card border-border">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold flex items-center gap-2">
                <ClipboardList className="w-5 h-5 text-primary" />
                Órdenes Recientes
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {recentOrders.map((order) => (
                  <div
                    key={order.id}
                    className="flex items-center justify-between p-3 rounded-lg bg-secondary/50 hover:bg-secondary transition-colors"
                  >
                    <div className="flex items-center gap-4">
                      <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary/10">
                        <ClipboardList className="w-5 h-5 text-primary" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">
                          {order.id} - {order.cliente}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {order.vehiculo}
                        </p>
                      </div>
                    </div>
                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${order.statusColor}`}>
                      {order.estado}
                    </span>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          {/* Low Stock Alert */}
          <Card className="bg-card border-border">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold flex items-center gap-2">
                <AlertTriangle className="w-5 h-5 text-warning" />
                Stock Bajo
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {lowStockItems.map((item) => (
                  <div
                    key={item.name}
                    className="p-3 rounded-lg bg-warning/5 border border-warning/20"
                  >
                    <p className="text-sm font-medium text-foreground">
                      {item.name}
                    </p>
                    <div className="flex items-center justify-between mt-2">
                      <span className="text-xs text-muted-foreground">
                        Stock: {item.stock} / Min: {item.minStock}
                      </span>
                      <span className="text-xs text-warning font-medium">
                        Reabastecer
                      </span>
                    </div>
                    <div className="mt-2 h-1.5 bg-secondary rounded-full overflow-hidden">
                      <div 
                        className="h-full bg-warning rounded-full"
                        style={{ width: `${(item.stock / item.minStock) * 100}%` }}
                      />
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Quick Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-success/10">
                  <CheckCircle className="w-6 h-6 text-success" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Completadas Hoy</p>
                  <p className="text-2xl font-bold text-foreground">5</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-primary/10">
                  <Clock className="w-6 h-6 text-primary" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Citas Hoy</p>
                  <p className="text-2xl font-bold text-foreground">8</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="bg-card border-border">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-success/10">
                  <DollarSign className="w-6 h-6 text-success" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Facturado Hoy</p>
                  <p className="text-2xl font-bold text-foreground">$2,450</p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </AppLayout>
  )
}
