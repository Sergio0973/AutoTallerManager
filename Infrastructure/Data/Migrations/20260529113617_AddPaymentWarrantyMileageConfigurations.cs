using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentWarrantyMileageConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantias_OrdenesServicio_OrdenId",
                table: "Garantias");

            migrationBuilder.DropForeignKey(
                name: "FK_Garantias_TiposServicio_TipoServicioId",
                table: "Garantias");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialesKilometraje_Vehiculos_VehiculoId",
                table: "HistorialesKilometraje");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_MetodosPago_MetodoPagoId",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialesKilometraje_VehiculoId",
                table: "HistorialesKilometraje");

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Pagos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Pagos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Fuente",
                table: "HistorialesKilometraje",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Garantias",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Condiciones",
                table: "Garantias",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_FechaPago",
                table: "Pagos",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_Referencia",
                table: "Pagos",
                column: "Referencia");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesKilometraje_VehiculoId_Fecha",
                table: "HistorialesKilometraje",
                columns: new[] { "VehiculoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Garantias_FechaVencimiento",
                table: "Garantias",
                column: "FechaVencimiento");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantias_OrdenesServicio_OrdenId",
                table: "Garantias",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Garantias_TiposServicio_TipoServicioId",
                table: "Garantias",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialesKilometraje_Vehiculos_VehiculoId",
                table: "HistorialesKilometraje",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_MetodosPago_MetodoPagoId",
                table: "Pagos",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantias_OrdenesServicio_OrdenId",
                table: "Garantias");

            migrationBuilder.DropForeignKey(
                name: "FK_Garantias_TiposServicio_TipoServicioId",
                table: "Garantias");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialesKilometraje_Vehiculos_VehiculoId",
                table: "HistorialesKilometraje");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_MetodosPago_MetodoPagoId",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_FechaPago",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_Referencia",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialesKilometraje_VehiculoId_Fecha",
                table: "HistorialesKilometraje");

            migrationBuilder.DropIndex(
                name: "IX_Garantias_FechaVencimiento",
                table: "Garantias");

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Pagos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Pagos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Fuente",
                table: "HistorialesKilometraje",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Garantias",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Condiciones",
                table: "Garantias",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesKilometraje_VehiculoId",
                table: "HistorialesKilometraje",
                column: "VehiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantias_OrdenesServicio_OrdenId",
                table: "Garantias",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Garantias_TiposServicio_TipoServicioId",
                table: "Garantias",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialesKilometraje_Vehiculos_VehiculoId",
                table: "HistorialesKilometraje",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_MetodosPago_MetodoPagoId",
                table: "Pagos",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
