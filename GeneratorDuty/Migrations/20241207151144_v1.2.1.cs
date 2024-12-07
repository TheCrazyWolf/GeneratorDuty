using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorDuty.Migrations
{
    /// <inheritdoc />
    public partial class v121 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_ScheduleProps_IdPeer",
                table: "History");

            migrationBuilder.DropIndex(
                name: "IX_History_IdPeer",
                table: "History");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_History_IdPeer",
                table: "History",
                column: "IdPeer");

            migrationBuilder.AddForeignKey(
                name: "FK_History_ScheduleProps_IdPeer",
                table: "History",
                column: "IdPeer",
                principalTable: "ScheduleProps",
                principalColumn: "Id");
        }
    }
}
