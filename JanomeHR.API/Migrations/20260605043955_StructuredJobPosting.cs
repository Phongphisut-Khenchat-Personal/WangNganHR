using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JanomeHR.API.Migrations
{
    public partial class StructuredJobPosting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "JobPostings",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Qualifications",
                table: "JobPostings",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "JobPostings",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "WorkHours",
                table: "JobPostings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkLocation",
                table: "JobPostings",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Benefits", table: "JobPostings");
            migrationBuilder.DropColumn(name: "Qualifications", table: "JobPostings");
            migrationBuilder.DropColumn(name: "Responsibilities", table: "JobPostings");
            migrationBuilder.DropColumn(name: "WorkHours", table: "JobPostings");
            migrationBuilder.DropColumn(name: "WorkLocation", table: "JobPostings");
        }
    }
}
