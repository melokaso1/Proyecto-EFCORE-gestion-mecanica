using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlacaVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Placa",
                table: "Vehiculos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Vehiculos"
                SET "Placa" = 'TMP' || "IdVehiculo"
                WHERE "Placa" IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Placa",
                table: "Vehiculos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Placa",
                table: "Vehiculos");
        }
    }
}
