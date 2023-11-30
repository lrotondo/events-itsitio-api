using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class live : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreamUrl",
                table: "Events",
                newName: "YoutubeStreamId");

            migrationBuilder.RenameColumn(
                name: "ChatUrl",
                table: "Events",
                newName: "ArenaChatName");

            migrationBuilder.AddColumn<bool>(
                name: "Live",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Live",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "YoutubeStreamId",
                table: "Events",
                newName: "StreamUrl");

            migrationBuilder.RenameColumn(
                name: "ArenaChatName",
                table: "Events",
                newName: "ChatUrl");
        }
    }
}
