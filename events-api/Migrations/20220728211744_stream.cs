using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class stream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YoutubeStreamId",
                table: "Events",
                newName: "StreamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreamId",
                table: "Events",
                newName: "YoutubeStreamId");
        }
    }
}
