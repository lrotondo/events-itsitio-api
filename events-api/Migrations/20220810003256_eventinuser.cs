using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class eventinuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersForEvents_Events_EventId",
                table: "UsersForEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersForEvents_Events_EventId",
                table: "UsersForEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersForEvents_Events_EventId",
                table: "UsersForEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersForEvents_Events_EventId",
                table: "UsersForEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
