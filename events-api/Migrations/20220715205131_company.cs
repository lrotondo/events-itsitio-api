using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Business",
                table: "UsersForEvents");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "UsersForEvents");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "UsersForEvents",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "UsersForEvents",
                newName: "Company");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "UsersForEvents",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "UsersForEvents",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "Business",
                table: "UsersForEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "UsersForEvents",
                type: "text",
                nullable: true);
        }
    }
}
