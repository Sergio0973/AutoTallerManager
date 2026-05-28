using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos",
                column: "Placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Vin",
                table: "Vehiculos",
                column: "Vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Abreviatura",
                table: "UnidadesMedida",
                column: "Abreviatura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Nombre",
                table: "UnidadesMedida",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposServicio_Nombre",
                table: "TiposServicio",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_Codigo",
                table: "Repuestos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetodosPago_Nombre",
                table: "MetodosPago",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosOrden_Nombre",
                table: "EstadosOrden",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosFactura_Nombre",
                table: "EstadosFactura",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DocumentoIdentidad",
                table: "Clientes",
                column: "DocumentoIdentidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasRepuesto_Nombre",
                table: "CategoriasRepuesto",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Vin",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesMedida_Abreviatura",
                table: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesMedida_Nombre",
                table: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_TiposServicio_Nombre",
                table: "TiposServicio");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Nombre",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Repuestos_Codigo",
                table: "Repuestos");

            migrationBuilder.DropIndex(
                name: "IX_MetodosPago_Nombre",
                table: "MetodosPago");

            migrationBuilder.DropIndex(
                name: "IX_EstadosOrden_Nombre",
                table: "EstadosOrden");

            migrationBuilder.DropIndex(
                name: "IX_EstadosFactura_Nombre",
                table: "EstadosFactura");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_DocumentoIdentidad",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasRepuesto_Nombre",
                table: "CategoriasRepuesto");
        }
    }
}
