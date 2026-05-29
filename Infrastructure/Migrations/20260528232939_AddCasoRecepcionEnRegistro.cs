using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCasoRecepcionEnRegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdVehiculo",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoServicio",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IdMecanico",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IdCliente",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "IdRecepcionista",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoIngreso",
                table: "OrdenesServicio",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.InsertData(
                table: "EstadosOrden",
                columns: new[] { "IdEstadoOrden", "Nombre" },
                values: new object[] { 18, "En registro" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdRecepcionista",
                table: "OrdenesServicio",
                column: "IdRecepcionista");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Usuarios_IdRecepcionista",
                table: "OrdenesServicio",
                column: "IdRecepcionista",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Usuarios_IdRecepcionista",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_IdRecepcionista",
                table: "OrdenesServicio");

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 18);

            migrationBuilder.DropColumn(
                name: "IdRecepcionista",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "MotivoIngreso",
                table: "OrdenesServicio");

            migrationBuilder.AlterColumn<int>(
                name: "IdVehiculo",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoServicio",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdMecanico",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdCliente",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
