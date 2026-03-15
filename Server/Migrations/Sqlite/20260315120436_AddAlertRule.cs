using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreConnect.Server.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddAlertRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceGroupCoreConnectUser");

            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: false),
                    Metric = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Operator = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Threshold = table.Column<double>(type: "REAL", nullable: false),
                    SavedScriptId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetDeviceGroupId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AlertRules_DeviceGroups_TargetDeviceGroupId",
                        column: x => x.TargetDeviceGroupId,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AlertRules_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AlertRules_SavedScripts_SavedScriptId",
                        column: x => x.SavedScriptId,
                        principalTable: "SavedScripts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoreConnectUserDeviceGroup",
                columns: table => new
                {
                    DeviceGroupsID = table.Column<string>(type: "TEXT", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreConnectUserDeviceGroup", x => new { x.DeviceGroupsID, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CoreConnectUserDeviceGroup_CoreConnectUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "CoreConnectUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoreConnectUserDeviceGroup_DeviceGroups_DeviceGroupsID",
                        column: x => x.DeviceGroupsID,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_OrganizationID",
                table: "AlertRules",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_SavedScriptId",
                table: "AlertRules",
                column: "SavedScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_TargetDeviceGroupId",
                table: "AlertRules",
                column: "TargetDeviceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CoreConnectUserDeviceGroup_UsersId",
                table: "CoreConnectUserDeviceGroup",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertRules");

            migrationBuilder.DropTable(
                name: "CoreConnectUserDeviceGroup");

            migrationBuilder.CreateTable(
                name: "DeviceGroupCoreConnectUser",
                columns: table => new
                {
                    DeviceGroupsID = table.Column<string>(type: "TEXT", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroupCoreConnectUser", x => new { x.DeviceGroupsID, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DeviceGroupCoreConnectUser_CoreConnectUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "CoreConnectUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceGroupCoreConnectUser_DeviceGroups_DeviceGroupsID",
                        column: x => x.DeviceGroupsID,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceGroupCoreConnectUser_UsersId",
                table: "DeviceGroupCoreConnectUser",
                column: "UsersId");
        }
    }
}
