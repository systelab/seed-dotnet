namespace main.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class seedr7v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "PatientAllergies",
                table => new
                {
                    IdAllergy = table.Column<Guid>(),
                    IdPatient = table.Column<Guid>(),
                    Id = table.Column<Guid>(),
                    CreationTime = table.Column<DateTime>(),
                    UpdateTime = table.Column<DateTime>(),
                    AllergiesId = table.Column<Guid>(nullable: true),
                    PatientsId = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(),
                    LastOcurrence = table.Column<DateTime>(),
                    AssertedDate = table.Column<DateTime>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergies", x => new {x.IdAllergy, x.IdPatient});
                    table.UniqueConstraint("AK_PatientAllergies_Id", x => x.Id);
                    table.ForeignKey(
                        "FK_PatientAllergies_Allergies_AllergiesId",
                        x => x.AllergiesId,
                        "Allergies",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PatientAllergies_Patients_PatientsId",
                        x => x.PatientsId,
                        "Patients",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_PatientAllergies_AllergiesId",
                "PatientAllergies",
                "AllergiesId");

            migrationBuilder.CreateIndex(
                "IX_PatientAllergies_PatientsId",
                "PatientAllergies",
                "PatientsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "PatientAllergies");
        }
    }
}