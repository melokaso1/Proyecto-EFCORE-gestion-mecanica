using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCostoAprobacionCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComentarioCliente",
                table: "OrdenesServicio",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CostoAprobado",
                table: "OrdenesServicio",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoPropuesto",
                table: "OrdenesServicio",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDecisionCosto",
                table: "OrdenesServicio",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotaCostoPropuesto",
                table: "OrdenesServicio",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "NombreRol" },
                values: new object[] { 4, "Cliente" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "ComentarioCliente",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "CostoAprobado",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "CostoPropuesto",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaDecisionCosto",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "NotaCostoPropuesto",
                table: "OrdenesServicio");
        }
    }
}
