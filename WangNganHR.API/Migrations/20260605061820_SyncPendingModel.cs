using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WangNganHR.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PdpaConsentedAt",
                table: "Applications",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferralSource",
                table: "Applications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdpaConsentedAt",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ReferralSource",
                table: "Applications");
        }
    }
}
