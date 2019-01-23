using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace main.Migrations
{
    public partial class seedr7v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientAllergies",
                columns: table => new
                {
                    IdAllergy = table.Column<Guid>(nullable: false),
                    IdPatient = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    AllergiesId = table.Column<Guid>(nullable: true),
                    PatientsId = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(nullable: false),
                    LastOcurrence = table.Column<DateTime>(nullable: false),
                    AssertedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergies", x => new { x.IdAllergy, x.IdPatient });
                    table.UniqueConstraint("AK_PatientAllergies_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_Allergies_AllergiesId",
                        column: x => x.AllergiesId,
                        principalTable: "Allergies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_Patients_PatientsId",
                        column: x => x.PatientsId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_AllergiesId",
                table: "PatientAllergies",
                column: "AllergiesId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_PatientsId",
                table: "PatientAllergies",
                column: "PatientsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientAllergies");
        }
    }
}
