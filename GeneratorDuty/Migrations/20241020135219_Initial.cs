using System;
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
                    IsAutoExport = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastResult = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleProps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogDutyMemberPriorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogDutyMemberPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogDutyMemberPriorities_MemberDuties_UserId",
                        column: x => x.UserId,
                        principalTable: "MemberDuties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LogDutyMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogDutyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogDutyMembers_MemberDuties_UserId",
                        column: x => x.UserId,
                        principalTable: "MemberDuties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogDutyMemberPriorities_UserId",
                table: "LogDutyMemberPriorities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogDutyMembers_UserId",
                table: "LogDutyMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogDutyMemberPriorities");

            migrationBuilder.DropTable(
                name: "LogDutyMembers");

            migrationBuilder.DropTable(
                name: "ScheduleProps");

            migrationBuilder.DropTable(
                name: "MemberDuties");
        }
    }
}
