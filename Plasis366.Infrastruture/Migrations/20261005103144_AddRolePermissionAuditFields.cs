using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plasis366.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissionAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RolePermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "RolePermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "RolePermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VersionNumber",
                table: "RolePermissions",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "RolePermissions");
        }
    }
}
