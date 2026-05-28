using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposVehiculo",
                columns: table => new
                {
                    IdTipoVehiculo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposVehiculo", x => x.IdTipoVehiculo);
                });

            migrationBuilder.InsertData(
                table: "TiposVehiculo",
                columns: new[] { "IdTipoVehiculo", "Nombre" },
                values: new object[,]
                {
                    { 1, "Sedán" },
                    { 2, "Hatchback" },
                    { 3, "SUV" },
                    { 4, "Camioneta" },
                    { 5, "Pickup" },
                    { 6, "Motocicleta" }
                });

            migrationBuilder.AddColumn<int>(
                name: "IdTipoVehiculo",
                table: "ModelosVehiculo",
                type: "integer",
                nullable: true);

            // Backfill existing rows before applying FK and NOT NULL constraint.
            // We default to "Sedán" (Id=1) for any existing model without a tipo.
            migrationBuilder.Sql("""
                UPDATE "ModelosVehiculo"
                SET "IdTipoVehiculo" = 1
                WHERE "IdTipoVehiculo" IS NULL OR "IdTipoVehiculo" = 0;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoVehiculo",
                table: "ModelosVehiculo",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelosVehiculo_IdTipoVehiculo",
                table: "ModelosVehiculo",
                column: "IdTipoVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_TiposVehiculo_Nombre",
                table: "TiposVehiculo",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelosVehiculo_TiposVehiculo_IdTipoVehiculo",
                table: "ModelosVehiculo",
                column: "IdTipoVehiculo",
                principalTable: "TiposVehiculo",
                principalColumn: "IdTipoVehiculo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelosVehiculo_TiposVehiculo_IdTipoVehiculo",
                table: "ModelosVehiculo");

            migrationBuilder.DropTable(
                name: "TiposVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_ModelosVehiculo_IdTipoVehiculo",
                table: "ModelosVehiculo");

            migrationBuilder.DropColumn(
                name: "IdTipoVehiculo",
                table: "ModelosVehiculo");
        }
    }
}
