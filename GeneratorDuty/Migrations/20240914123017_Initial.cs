using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorDuty.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberDuties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPeer = table.Column<long>(type: "INTEGER", nullable: false),
                    MemberNameDuty = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberDuties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleProps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPeer = table.Column<long>(type: "INTEGER", nullable: false),
                    SearchType = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsAutoSend = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastResult = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleProps", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberDuties");

            migrationBuilder.DropTable(
                name: "ScheduleProps");
        }
    }
}
