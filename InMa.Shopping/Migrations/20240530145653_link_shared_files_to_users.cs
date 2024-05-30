using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMa.Shopping.Migrations
{
    /// <inheritdoc />
    public partial class link_shared_files_to_users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedFilesUsersLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SharedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedFilesUsersLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedFilesUsersLink_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedFilesUsersLink_SharedFiles_SharedFileId",
                        column: x => x.SharedFileId,
                        principalTable: "SharedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedFilesUsersLink_SharedFileId",
                table: "SharedFilesUsersLink",
                column: "SharedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFilesUsersLink_UserId",
                table: "SharedFilesUsersLink",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedFilesUsersLink");
        }
    }
}
