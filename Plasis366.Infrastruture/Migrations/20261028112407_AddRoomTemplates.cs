using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plasis366.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RoomTemplateId",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomTemplate",
                columns: table => new
                {
                    RoomTemplateId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Style = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VersionNumber = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTemplate", x => x.RoomTemplateId);
                    table.ForeignKey(
                        name: "FK_RoomTemplate_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomTemplateImage",
                columns: table => new
                {
                    RoomTemplateImageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VersionNumber = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTemplateImage", x => x.RoomTemplateImageId);
                    table.ForeignKey(
                        name: "FK_RoomTemplateImage_RoomTemplate_RoomTemplateId",
                        column: x => x.RoomTemplateId,
                        principalTable: "RoomTemplate",
                        principalColumn: "RoomTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTemplateId",
                table: "Rooms",
                column: "RoomTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTemplate_TenantId",
                table: "RoomTemplate",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTemplateImage_RoomTemplateId",
                table: "RoomTemplateImage",
                column: "RoomTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTemplate_RoomTemplateId",
                table: "Rooms",
                column: "RoomTemplateId",
                principalTable: "RoomTemplate",
                principalColumn: "RoomTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTemplate_RoomTemplateId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomTemplateImage");

            migrationBuilder.DropTable(
                name: "RoomTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTemplateId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomTemplateId",
                table: "Rooms");
        }
    }
}
