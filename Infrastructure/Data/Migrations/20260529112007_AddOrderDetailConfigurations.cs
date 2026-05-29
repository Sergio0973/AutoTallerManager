using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialEstadosOrden_EstadosOrden_EstadoId",
                table: "HistorialEstadosOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTiposServicio_TiposServicio_TipoServicioId",
                table: "OrdenesTiposServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_TareasMecanicos_TiposServicio_TipoServicioId",
                table: "TareasMecanicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTiposServicio_OrdenId",
                table: "OrdenesTiposServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesMecanicos_OrdenId",
                table: "OrdenesMecanicos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialEstadosOrden_OrdenId",
                table: "HistorialEstadosOrden");

            migrationBuilder.DropIndex(
                name: "IX_DetallesOrden_OrdenId",
                table: "DetallesOrden");

            migrationBuilder.AlterColumn<decimal>(
                name: "HorasTrabajadas",
                table: "TareasMecanicos",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "TareasMecanicos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TareasMecanicos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoHora",
                table: "TareasMecanicos",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "NotasOrden",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "HistorialEstadosOrden",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioSnapshot",
                table: "DetallesOrden",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTiposServicio_OrdenId_TipoServicioId",
                table: "OrdenesTiposServicio",
                columns: new[] { "OrdenId", "TipoServicioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMecanicos_OrdenId_MecanicoId",
                table: "OrdenesMecanicos",
                columns: new[] { "OrdenId", "MecanicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosOrden_OrdenId_FechaCambio",
                table: "HistorialEstadosOrden",
                columns: new[] { "OrdenId", "FechaCambio" });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrden_OrdenId_RepuestoId",
                table: "DetallesOrden",
                columns: new[] { "OrdenId", "RepuestoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialEstadosOrden_EstadosOrden_EstadoId",
                table: "HistorialEstadosOrden",
                column: "EstadoId",
                principalTable: "EstadosOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTiposServicio_TiposServicio_TipoServicioId",
                table: "OrdenesTiposServicio",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TareasMecanicos_TiposServicio_TipoServicioId",
                table: "TareasMecanicos",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialEstadosOrden_EstadosOrden_EstadoId",
                table: "HistorialEstadosOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTiposServicio_TiposServicio_TipoServicioId",
                table: "OrdenesTiposServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_TareasMecanicos_TiposServicio_TipoServicioId",
                table: "TareasMecanicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTiposServicio_OrdenId_TipoServicioId",
                table: "OrdenesTiposServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesMecanicos_OrdenId_MecanicoId",
                table: "OrdenesMecanicos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialEstadosOrden_OrdenId_FechaCambio",
                table: "HistorialEstadosOrden");

            migrationBuilder.DropIndex(
                name: "IX_DetallesOrden_OrdenId_RepuestoId",
                table: "DetallesOrden");

            migrationBuilder.AlterColumn<decimal>(
                name: "HorasTrabajadas",
                table: "TareasMecanicos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "TareasMecanicos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TareasMecanicos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoHora",
                table: "TareasMecanicos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "NotasOrden",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "HistorialEstadosOrden",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioSnapshot",
                table: "DetallesOrden",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTiposServicio_OrdenId",
                table: "OrdenesTiposServicio",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMecanicos_OrdenId",
                table: "OrdenesMecanicos",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosOrden_OrdenId",
                table: "HistorialEstadosOrden",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrden_OrdenId",
                table: "DetallesOrden",
                column: "OrdenId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialEstadosOrden_EstadosOrden_EstadoId",
                table: "HistorialEstadosOrden",
                column: "EstadoId",
                principalTable: "EstadosOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTiposServicio_TiposServicio_TipoServicioId",
                table: "OrdenesTiposServicio",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TareasMecanicos_TiposServicio_TipoServicioId",
                table: "TareasMecanicos",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
