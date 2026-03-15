using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreConnect.Server.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddOrgAllowedIpRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedIpRanges",
                table: "Organizations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedIpRanges",
                table: "Organizations");
        }
    }
}
