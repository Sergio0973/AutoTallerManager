using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryPurchaseConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Proveedores_ProveedorId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsInventario_Compras_CompraId",
                table: "LogsInventario");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsInventario_OrdenesServicio_OrdenId",
                table: "LogsInventario");

            migrationBuilder.DropIndex(
                name: "IX_RepuestosProveedor_RepuestoId",
                table: "RepuestosProveedor");

            migrationBuilder.DropIndex(
                name: "IX_LogsInventario_RepuestoId",
                table: "LogsInventario");

            migrationBuilder.DropIndex(
                name: "IX_DetallesCompra_CompraId",
                table: "DetallesCompra");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioCompra",
                table: "RepuestosProveedor",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Proveedores",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nit",
                table: "Proveedores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Proveedores",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TipoMovimiento",
                table: "LogsInventario",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "LogsInventario",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "DetallesCompra",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Compras",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Compras",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_RepuestosProveedor_RepuestoId_ProveedorId",
                table: "RepuestosProveedor",
                columns: new[] { "RepuestoId", "ProveedorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Correo",
                table: "Proveedores",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Nit",
                table: "Proveedores",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogsInventario_RepuestoId_Fecha",
                table: "LogsInventario",
                columns: new[] { "RepuestoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompra_CompraId_RepuestoId",
                table: "DetallesCompra",
                columns: new[] { "CompraId", "RepuestoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_FechaCompra",
                table: "Compras",
                column: "FechaCompra");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Proveedores_ProveedorId",
                table: "Compras",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsInventario_Compras_CompraId",
                table: "LogsInventario",
                column: "CompraId",
                principalTable: "Compras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsInventario_OrdenesServicio_OrdenId",
                table: "LogsInventario",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Proveedores_ProveedorId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsInventario_Compras_CompraId",
                table: "LogsInventario");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsInventario_OrdenesServicio_OrdenId",
                table: "LogsInventario");

            migrationBuilder.DropIndex(
                name: "IX_RepuestosProveedor_RepuestoId_ProveedorId",
                table: "RepuestosProveedor");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_Correo",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_Nit",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_LogsInventario_RepuestoId_Fecha",
                table: "LogsInventario");

            migrationBuilder.DropIndex(
                name: "IX_DetallesCompra_CompraId_RepuestoId",
                table: "DetallesCompra");

            migrationBuilder.DropIndex(
                name: "IX_Compras_FechaCompra",
                table: "Compras");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioCompra",
                table: "RepuestosProveedor",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Proveedores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Nit",
                table: "Proveedores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Proveedores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "TipoMovimiento",
                table: "LogsInventario",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "LogsInventario",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "DetallesCompra",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Compras",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Compras",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_RepuestosProveedor_RepuestoId",
                table: "RepuestosProveedor",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsInventario_RepuestoId",
                table: "LogsInventario",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompra_CompraId",
                table: "DetallesCompra",
                column: "CompraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Proveedores_ProveedorId",
                table: "Compras",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsInventario_Compras_CompraId",
                table: "LogsInventario",
                column: "CompraId",
                principalTable: "Compras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsInventario_OrdenesServicio_OrdenId",
                table: "LogsInventario",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id");
        }
    }
}
