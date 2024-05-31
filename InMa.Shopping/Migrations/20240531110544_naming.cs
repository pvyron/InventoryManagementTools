using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMa.Shopping.Migrations
{
    /// <inheritdoc />
    public partial class naming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFilesUsersLinkDbModel_AspNetUsers_UserId",
                table: "SharedFilesUsersLinkDbModel");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFilesUsersLinkDbModel_SharedFiles_SharedFileId",
                table: "SharedFilesUsersLinkDbModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedFilesUsersLinkDbModel",
                table: "SharedFilesUsersLinkDbModel");

            migrationBuilder.RenameTable(
                name: "SharedFilesUsersLinkDbModel",
                newName: "SharedFilesUsersLinks");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFilesUsersLinkDbModel_UserId",
                table: "SharedFilesUsersLinks",
                newName: "IX_SharedFilesUsersLinks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFilesUsersLinkDbModel_SharedFileId",
                table: "SharedFilesUsersLinks",
                newName: "IX_SharedFilesUsersLinks_SharedFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedFilesUsersLinks",
                table: "SharedFilesUsersLinks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFilesUsersLinks_AspNetUsers_UserId",
                table: "SharedFilesUsersLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFilesUsersLinks_SharedFiles_SharedFileId",
                table: "SharedFilesUsersLinks",
                column: "SharedFileId",
                principalTable: "SharedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFilesUsersLinks_AspNetUsers_UserId",
                table: "SharedFilesUsersLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFilesUsersLinks_SharedFiles_SharedFileId",
                table: "SharedFilesUsersLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedFilesUsersLinks",
                table: "SharedFilesUsersLinks");

            migrationBuilder.RenameTable(
                name: "SharedFilesUsersLinks",
                newName: "SharedFilesUsersLinkDbModel");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFilesUsersLinks_UserId",
                table: "SharedFilesUsersLinkDbModel",
                newName: "IX_SharedFilesUsersLinkDbModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFilesUsersLinks_SharedFileId",
                table: "SharedFilesUsersLinkDbModel",
                newName: "IX_SharedFilesUsersLinkDbModel_SharedFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedFilesUsersLinkDbModel",
                table: "SharedFilesUsersLinkDbModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFilesUsersLinkDbModel_AspNetUsers_UserId",
                table: "SharedFilesUsersLinkDbModel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFilesUsersLinkDbModel_SharedFiles_SharedFileId",
                table: "SharedFilesUsersLinkDbModel",
                column: "SharedFileId",
                principalTable: "SharedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
