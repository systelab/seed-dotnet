namespace main.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class seed_db_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                "CreationTime",
                "Patients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                "UpdateTime",
                "Patients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                "CreationTime",
                "Address",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                "UpdateTime",
                "Address",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "CreationTime",
                "Patients");

            migrationBuilder.DropColumn(
                "UpdateTime",
                "Patients");

            migrationBuilder.DropColumn(
                "CreationTime",
                "Address");

            migrationBuilder.DropColumn(
                "UpdateTime",
                "Address");
        }
    }
}