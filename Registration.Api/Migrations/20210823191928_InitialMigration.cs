using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Registration.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserRole = table.Column<string>(type: "TEXT", nullable: true),
                    UserRegEmail = table.Column<string>(type: "TEXT", nullable: true),
                    UserMSTeamsEmail = table.Column<string>(type: "TEXT", nullable: true),
                    UserDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    MySkills = table.Column<string>(type: "TEXT", nullable: true),
                    UserTimeCommitment = table.Column<string>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ADUserId = table.Column<string>(type: "TEXT", nullable: true),
                    UserOptOut = table.Column<bool>(type: "INTEGER", nullable: false),
                    MSFTOptIn = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
