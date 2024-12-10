using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorDuty.Migrations
{
    /// <inheritdoc />
    public partial class v132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPeer",
                table: "History");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "History");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdPeer",
                table: "History",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "History",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
