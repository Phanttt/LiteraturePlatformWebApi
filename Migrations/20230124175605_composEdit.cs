using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteraturePlatformWebApi.Migrations
{
    /// <inheritdoc />
    public partial class composEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Composition",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Composition",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Composition");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Composition",
                newName: "Name");
        }
    }
}
