using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events_api.Migrations
{
    public partial class eventsrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Speakers_Events_EventId",
                table: "Speakers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sponsors_Events_EventId",
                table: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_Sponsors_EventId",
                table: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_EventId",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Sponsors");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Speakers");

            migrationBuilder.CreateTable(
                name: "EventSponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SponsorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSponsors_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSponsors_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventsSpeakers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsSpeakers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventsSpeakers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventsSpeakers_Speakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "Speakers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSponsors_EventId",
                table: "EventSponsors",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSponsors_SponsorId",
                table: "EventSponsors",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsSpeakers_EventId",
                table: "EventsSpeakers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsSpeakers_SpeakerId",
                table: "EventsSpeakers",
                column: "SpeakerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventSponsors");

            migrationBuilder.DropTable(
                name: "EventsSpeakers");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Sponsors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Speakers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_EventId",
                table: "Sponsors",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_EventId",
                table: "Speakers",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Speakers_Events_EventId",
                table: "Speakers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sponsors_Events_EventId",
                table: "Sponsors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
