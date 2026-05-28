using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenClienteYJefeBodega : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "OrdenesServicio"
                SET "IdCliente" = (
                    SELECT h."IdCliente"
                    FROM "HistorialPropietariosVehiculo" h
                    WHERE h."IdVehiculo" = "OrdenesServicio"."IdVehiculo"
                      AND h."FechaFin" IS NULL
                    ORDER BY h."FechaInicio" DESC
                    LIMIT 1
                )
                WHERE "IdCliente" IS NULL OR "IdCliente" = 0;
                """);

            migrationBuilder.Sql("""
                UPDATE "OrdenesServicio"
                SET "IdCliente" = (
                    SELECT c."IdCliente"
                    FROM "Clientes" c
                    ORDER BY c."IdCliente"
                    LIMIT 1
                )
                WHERE "IdCliente" IS NULL OR "IdCliente" = 0;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "IdCliente",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "NombreRol" },
                values: new object[] { 6, "JefeBodega" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdCliente",
                table: "OrdenesServicio",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Clientes_IdCliente",
                table: "OrdenesServicio",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Clientes_IdCliente",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_IdCliente",
                table: "OrdenesServicio");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "OrdenesServicio");
        }
    }
}
