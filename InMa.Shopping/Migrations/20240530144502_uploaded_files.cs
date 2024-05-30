using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMa.Shopping.Migrations
{
    /// <inheritdoc />
    public partial class uploaded_files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SharedFiles_SharedFileDbModelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_AspNetUsers_UploaderId",
                table: "SharedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SharedFileDbModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SharedFileDbModelId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UploaderId",
                table: "SharedFiles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCaptured",
                table: "SharedFiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "SharedFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "SharedFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_AspNetUsers_UploaderId",
                table: "SharedFiles",
                column: "UploaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_AspNetUsers_UploaderId",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "DateCaptured",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "SharedFiles");

            migrationBuilder.AlterColumn<string>(
                name: "UploaderId",
                table: "SharedFiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "SharedFileDbModelId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SharedFileDbModelId",
                table: "AspNetUsers",
                column: "SharedFileDbModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SharedFiles_SharedFileDbModelId",
                table: "AspNetUsers",
                column: "SharedFileDbModelId",
                principalTable: "SharedFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_AspNetUsers_UploaderId",
                table: "SharedFiles",
                column: "UploaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
