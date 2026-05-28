using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticoYReparaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiagnosticosOrden",
                columns: table => new
                {
                    IdDiagnosticoOrden = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false),
                    IdMecanico = table.Column<int>(type: "integer", nullable: false),
                    FechaDiagnostico = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SintomasReportados = table.Column<string>(type: "text", nullable: true),
                    Hallazgos = table.Column<string>(type: "text", nullable: true),
                    Recomendaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticosOrden", x => x.IdDiagnosticoOrden);
                    table.ForeignKey(
                        name: "FK_DiagnosticosOrden_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosticosOrden_Usuarios_IdMecanico",
                        column: x => x.IdMecanico,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReparacionesItem",
                columns: table => new
                {
                    IdReparacionItem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false),
                    IdMecanico = table.Column<int>(type: "integer", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CostoEstimado = table.Column<decimal>(type: "numeric", nullable: false),
                    HorasManoObraEstimada = table.Column<decimal>(type: "numeric", nullable: true),
                    Estado = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    AprobadoPorJefe = table.Column<bool>(type: "boolean", nullable: true),
                    FechaDecisionJefe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ComentarioJefe = table.Column<string>(type: "text", nullable: true),
                    AprobadoPorCliente = table.Column<bool>(type: "boolean", nullable: true),
                    FechaDecisionCliente = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ComentarioCliente = table.Column<string>(type: "text", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReparacionesItem", x => x.IdReparacionItem);
                    table.ForeignKey(
                        name: "FK_ReparacionesItem_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReparacionesItem_Usuarios_IdMecanico",
                        column: x => x.IdMecanico,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EstadosOrden",
                columns: new[] { "IdEstadoOrden", "Nombre" },
                values: new object[,]
                {
                    { 5, "Recibido" },
                    { 6, "Diagnóstico en proceso" },
                    { 7, "Pendiente aprobación jefe" },
                    { 8, "Pendiente aprobación cliente" },
                    { 9, "Rechazado por cliente" },
                    { 10, "Aprobado parcial" },
                    { 11, "Aprobado total" },
                    { 12, "Reparación en proceso" },
                    { 13, "Listo para entrega" },
                    { 14, "Pendiente de pago" },
                    { 15, "Pagado" },
                    { 16, "Entregado" },
                    { 17, "Cerrado" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "NombreRol" },
                values: new object[] { 5, "JefeMecanicos" });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticosOrden_IdMecanico",
                table: "DiagnosticosOrden",
                column: "IdMecanico");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticosOrden_IdOrdenServicio",
                table: "DiagnosticosOrden",
                column: "IdOrdenServicio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReparacionesItem_IdMecanico",
                table: "ReparacionesItem",
                column: "IdMecanico");

            migrationBuilder.CreateIndex(
                name: "IX_ReparacionesItem_IdOrdenServicio_Orden",
                table: "ReparacionesItem",
                columns: new[] { "IdOrdenServicio", "Orden" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosticosOrden");

            migrationBuilder.DropTable(
                name: "ReparacionesItem");

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "EstadosOrden",
                keyColumn: "IdEstadoOrden",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 5);
        }
    }
}
