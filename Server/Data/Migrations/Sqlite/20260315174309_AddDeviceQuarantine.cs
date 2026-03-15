using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreConnect.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddDeviceQuarantine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Devices");
        }
    }
}
