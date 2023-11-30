using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class primarycolor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionColor",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "TitleColor",
                table: "Events",
                newName: "PrimaryColor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryColor",
                table: "Events",
                newName: "TitleColor");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionColor",
                table: "Events",
                type: "text",
                nullable: true);
        }
    }
}
