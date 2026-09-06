using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace consultorio_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProfileDNI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DNI",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "DNI",
                table: "Patients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DNI",
                table: "Psychologists",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DNI",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
