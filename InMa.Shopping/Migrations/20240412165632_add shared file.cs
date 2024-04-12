using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMa.Shopping.Migrations
{
    /// <inheritdoc />
    public partial class addsharedfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SharedFileDbModelId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SharedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    UploadedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UploaderId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedFiles_AspNetUsers_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SharedFileDbModelId",
                table: "AspNetUsers",
                column: "SharedFileDbModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_UploaderId",
                table: "SharedFiles",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SharedFiles_SharedFileDbModelId",
                table: "AspNetUsers",
                column: "SharedFileDbModelId",
                principalTable: "SharedFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SharedFiles_SharedFileDbModelId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SharedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SharedFileDbModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SharedFileDbModelId",
                table: "AspNetUsers");
        }
    }
}
