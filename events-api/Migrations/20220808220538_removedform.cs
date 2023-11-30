using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class removedform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArenaChatName",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "FormUrl",
                table: "Events",
                newName: "ArenaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArenaId",
                table: "Events",
                newName: "FormUrl");

            migrationBuilder.AddColumn<string>(
                name: "ArenaChatName",
                table: "Events",
                type: "text",
                nullable: true);
        }
    }
}
