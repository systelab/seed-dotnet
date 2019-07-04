namespace main.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class seedr7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Allergies",
                table => new
                {
                    Id = table.Column<Guid>(),
                    CreationTime = table.Column<DateTime>(),
                    UpdateTime = table.Column<DateTime>(),
                    Name = table.Column<string>(maxLength: 255),
                    Signs = table.Column<string>(maxLength: 255),
                    Symptoms = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Allergies", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Allergies");
        }
    }
}