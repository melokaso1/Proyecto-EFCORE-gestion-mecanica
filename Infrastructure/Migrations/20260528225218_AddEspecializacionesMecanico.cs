using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEspecializacionesMecanico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EspecializacionesMecanico",
                columns: table => new
                {
                    IdEspecializacionMecanico = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecializacionesMecanico", x => x.IdEspecializacionMecanico);
                });

            migrationBuilder.InsertData(
                table: "EspecializacionesMecanico",
                columns: new[] { "IdEspecializacionMecanico", "Activo", "Codigo", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "DIAGNOSTICO", "Evaluación y detección de fallas", "Diagnóstico" },
                    { 2, true, "MOTOR", "Reparación de motor y componentes internos", "Motor" },
                    { 3, true, "FRENOS", "Sistema de frenos y ABS", "Frenos" },
                    { 4, true, "SUSPENSION", "Suspensión, dirección y alineación", "Suspensión" },
                    { 5, true, "ELECTRICO", "Batería, alternador, cableado y electrónica", "Sistema eléctrico" },
                    { 6, true, "TRANSMISION", "Caja manual/automática y embrague", "Transmisión" },
                    { 7, true, "AIRE_AC", "Climatización y refrigeración", "Aire acondicionado" },
                    { 8, true, "CARROCERIA", "Latonería, pintura y estética", "Carrocería" },
                    { 9, true, "GENERAL", "Aceite, filtros y servicios preventivos", "Mantenimiento general" }
                });

            migrationBuilder.CreateTable(
                name: "MecanicoEspecializaciones",
                columns: table => new
                {
                    EspecializacionesIdEspecializacionMecanico = table.Column<int>(type: "integer", nullable: false),
                    MecanicosIdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MecanicoEspecializaciones", x => new { x.EspecializacionesIdEspecializacionMecanico, x.MecanicosIdUsuario });
                    table.ForeignKey(
                        name: "FK_MecanicoEspecializaciones_EspecializacionesMecanico_Especia~",
                        column: x => x.EspecializacionesIdEspecializacionMecanico,
                        principalTable: "EspecializacionesMecanico",
                        principalColumn: "IdEspecializacionMecanico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MecanicoEspecializaciones_Usuarios_MecanicosIdUsuario",
                        column: x => x.MecanicosIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<int>(
                name: "IdEspecializacionMecanico",
                table: "ReparacionesItem",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "ReparacionesItem"
                SET "IdEspecializacionMecanico" = 9
                WHERE "IdEspecializacionMecanico" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "IdEspecializacionMecanico",
                table: "ReparacionesItem",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReparacionesItem_IdEspecializacionMecanico",
                table: "ReparacionesItem",
                column: "IdEspecializacionMecanico");

            migrationBuilder.CreateIndex(
                name: "IX_EspecializacionesMecanico_Codigo",
                table: "EspecializacionesMecanico",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MecanicoEspecializaciones_MecanicosIdUsuario",
                table: "MecanicoEspecializaciones",
                column: "MecanicosIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_ReparacionesItem_EspecializacionesMecanico_IdEspecializacio~",
                table: "ReparacionesItem",
                column: "IdEspecializacionMecanico",
                principalTable: "EspecializacionesMecanico",
                principalColumn: "IdEspecializacionMecanico",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReparacionesItem_EspecializacionesMecanico_IdEspecializacio~",
                table: "ReparacionesItem");

            migrationBuilder.DropTable(
                name: "MecanicoEspecializaciones");

            migrationBuilder.DropTable(
                name: "EspecializacionesMecanico");

            migrationBuilder.DropIndex(
                name: "IX_ReparacionesItem_IdEspecializacionMecanico",
                table: "ReparacionesItem");

            migrationBuilder.DropColumn(
                name: "IdEspecializacionMecanico",
                table: "ReparacionesItem");
        }
    }
}
