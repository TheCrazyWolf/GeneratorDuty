using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorDuty.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMigrated",
                table: "ScheduleProps",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPeer = table.Column<long>(type: "INTEGER", nullable: true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_ScheduleProps_IdPeer",
                        column: x => x.IdPeer,
                        principalTable: "ScheduleProps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_IdPeer",
                table: "History",
                column: "IdPeer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropColumn(
                name: "IsMigrated",
                table: "ScheduleProps");
        }
    }
}
