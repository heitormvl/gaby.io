using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gaby.io.Migrations
{
    /// <inheritdoc />
    public partial class AddReadingDetailsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Readings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PagesRead",
                table: "Readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Readings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Readings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "PagesRead",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Readings");
        }
    }
}
