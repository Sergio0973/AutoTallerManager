using Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(AutoTallerDbContext))]
    [Migration("20260528140000_SeedReferenceData")]
    public sealed class SeedReferenceData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columnTypes: new[] { "integer", "text", "text" },
                columns: new[] { "Id", "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { 1, "ADMIN", "Administrador del sistema" },
                    { 2, "RECEPCIONISTA", "Recepcion de vehiculos y ordenes" },
                    { 3, "MECANICO", "Ejecucion tecnica de servicios" }
                });

            migrationBuilder.InsertData(
                table: "EstadosFactura",
                columnTypes: new[] { "integer", "text" },
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "PENDIENTE" },
                    { 2, "PAGADA" },
                    { 3, "ANULADA" }
                });

            migrationBuilder.InsertData(
                table: "EstadosOrden",
                columnTypes: new[] { "integer", "text", "text" },
                columns: new[] { "Id", "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { 1, "RECIBIDA", "Orden creada y pendiente de diagnostico" },
                    { 2, "DIAGNOSTICO", "Vehiculo en revision tecnica" },
                    { 3, "REPARACION", "Trabajo mecanico en ejecucion" },
                    { 4, "LISTA", "Servicio finalizado y pendiente de entrega" },
                    { 5, "ENTREGADA", "Vehiculo entregado al cliente" },
                    { 6, "CANCELADA", "Orden cancelada" }
                });

            migrationBuilder.InsertData(
                table: "MetodosPago",
                columnTypes: new[] { "integer", "text", "text" },
                columns: new[] { "Id", "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { 1, "EFECTIVO", "Pago en efectivo" },
                    { 2, "TARJETA", "Pago con tarjeta debito o credito" },
                    { 3, "TRANSFERENCIA", "Pago mediante transferencia bancaria" }
                });

            migrationBuilder.InsertData(
                table: "UnidadesMedida",
                columnTypes: new[] { "integer", "text", "text" },
                columns: new[] { "Id", "Nombre", "Abreviatura" },
                values: new object[,]
                {
                    { 1, "UNIDAD", "UND" },
                    { 2, "LITRO", "LT" },
                    { 3, "JUEGO", "JGO" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasRepuesto",
                columnTypes: new[] { "integer", "text", "text" },
                columns: new[] { "Id", "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { 1, "MOTOR", "Repuestos y consumibles del motor" },
                    { 2, "FRENOS", "Partes del sistema de frenado" },
                    { 3, "SUSPENSION", "Elementos del sistema de suspension" },
                    { 4, "ELECTRICIDAD", "Componentes electricos y electronicos" }
                });

            migrationBuilder.InsertData(
                table: "TiposServicio",
                columnTypes: new[] { "integer", "text", "text", "integer" },
                columns: new[] { "Id", "Nombre", "Descripcion", "DiasEstimados" },
                values: new object[,]
                {
                    { 1, "CAMBIO_DE_ACEITE", "Mantenimiento preventivo general", 1 },
                    { 2, "REVISION_DE_FRENOS", "Diagnostico y ajuste del sistema de frenos", 1 },
                    { 3, "ALINEACION_Y_BALANCEO", "Correccion de direccion y rodamiento", 1 },
                    { 4, "MANTENIMIENTO_CORRECTIVO", "Correccion de fallas mecanicas", 3 }
                });

            migrationBuilder.InsertData(
                table: "MarcasVehiculo",
                columnTypes: new[] { "integer", "text" },
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "GENERICA" });

            migrationBuilder.InsertData(
                table: "ModelosVehiculo",
                columnTypes: new[] { "integer", "integer", "character varying(100)", "smallint", "smallint" },
                columns: new[] { "Id", "MarcaId", "Nombre", "AnioDesde", "AnioHasta" },
                values: new object[] { 1, 1, "BASE", (short)2000, (short)2035 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "ModelosVehiculo", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "MarcasVehiculo", keyColumn: "Id", keyValue: 1);

            migrationBuilder.DeleteData(table: "TiposServicio", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "TiposServicio", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "TiposServicio", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "TiposServicio", keyColumn: "Id", keyValue: 4);

            migrationBuilder.DeleteData(table: "CategoriasRepuesto", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "CategoriasRepuesto", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "CategoriasRepuesto", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "CategoriasRepuesto", keyColumn: "Id", keyValue: 4);

            migrationBuilder.DeleteData(table: "UnidadesMedida", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "UnidadesMedida", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "UnidadesMedida", keyColumn: "Id", keyValue: 3);

            migrationBuilder.DeleteData(table: "MetodosPago", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "MetodosPago", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "MetodosPago", keyColumn: "Id", keyValue: 3);

            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "EstadosOrden", keyColumn: "Id", keyValue: 6);

            migrationBuilder.DeleteData(table: "EstadosFactura", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "EstadosFactura", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "EstadosFactura", keyColumn: "Id", keyValue: 3);

            migrationBuilder.DeleteData(table: "Roles", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Roles", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Roles", keyColumn: "Id", keyValue: 3);
        }
    }
}
