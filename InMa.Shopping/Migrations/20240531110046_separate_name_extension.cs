using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMa.Shopping.Migrations
{
    /// <inheritdoc />
    public partial class separate_name_extension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "SharedFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "SharedFiles");
        }
    }
}
