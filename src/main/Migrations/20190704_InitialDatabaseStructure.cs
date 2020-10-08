namespace Main.Migrations
{
    using System.Data;

    using FluentMigrator;
    using FluentMigrator.Builders.Create.Table;

    /// <summary>
    ///     Initial database structure creation
    /// </summary>
    [Migration(20190704162500)]
    public class InitialDatabaseStructure : Migration
    {
        /// <summary>
        /// Down method
        /// </summary>
        public override void Down()
        {
            //This method is empty because this script is the initial creation of the database
        }

        public override void Up()
        {
            string creationTimeColumnName = "CreationTime";
            string updateTimeColumnName = "UpdateTime";

            string addressTableName = "Address";
            string allergiesTableName = "Allergies";
            string patientsTableName = "Patients";
            string patientAllergiesTableName = "PatientAllergies";
            string aspNetRolesTableName = "AspNetRoles";
            string aspNetUsersTableName = "AspNetUsers";
            string aspNetRoleClaimsTableName = "AspNetRoleClaims";
            string aspNetUsersClaimsTableName = "AspNetUserClaims";
            string aspNetUserLoginsTableName = "AspNetUserLogins";
            string aspNetUserRolesTableName = "AspNetUserRoles";
            string aspNetUserTokensTableName = "AspNetUserTokens";

            if (!this.Schema.Table(addressTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax addressTable = this.Create.Table(addressTableName);
                addressTable.WithColumn("Id").AsBinary().PrimaryKey().NotNullable();
                addressTable.WithColumn("City").AsString().Nullable();
                addressTable.WithColumn("Coordinates").AsString().Nullable();
                addressTable.WithColumn("Street").AsString().Nullable();
                addressTable.WithColumn("Zip").AsString().Nullable();
                addressTable.WithColumn(creationTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                addressTable.WithColumn(updateTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
            }

            if (!this.Schema.Table(allergiesTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax allergiesTable = this.Create.Table(allergiesTableName);
                allergiesTable.WithColumn("Id").AsBinary().PrimaryKey().NotNullable();
                allergiesTable.WithColumn(creationTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                allergiesTable.WithColumn(updateTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                allergiesTable.WithColumn("Name").AsString().NotNullable();
                allergiesTable.WithColumn("Signs").AsString().NotNullable();
                allergiesTable.WithColumn("Symptoms").AsString().Nullable();
            }

            if (!this.Schema.Table(patientsTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax patientsTable = this.Create.Table(patientsTableName);
                patientsTable.WithColumn("Id").AsBinary().PrimaryKey().NotNullable();
                patientsTable.WithColumn("AddressId").AsBinary().ForeignKey(addressTableName, "Id").Nullable();
                patientsTable.WithColumn("Dob").AsString().NotNullable();
                patientsTable.WithColumn("Email").AsString().Nullable();
                patientsTable.WithColumn("Name").AsString().Nullable();
                patientsTable.WithColumn("Surname").AsString().Nullable();
                patientsTable.WithColumn("MedicalNumber").AsString().Nullable();
                patientsTable.WithColumn(creationTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                patientsTable.WithColumn(updateTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
            }

            if (!this.Schema.Table(patientAllergiesTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax patientAllergiesTable = this.Create.Table(patientAllergiesTableName);
                patientAllergiesTable.WithColumn("IdAllergy").AsBinary().Nullable();
                patientAllergiesTable.WithColumn("IdPatient").AsBinary().Nullable();
                patientAllergiesTable.WithColumn("Id").AsBinary().Unique().NotNullable();
                patientAllergiesTable.WithColumn(creationTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                patientAllergiesTable.WithColumn(updateTimeColumnName).AsDateTime().NotNullable().WithDefaultValue("0001-01-01 00:00:00");
                patientAllergiesTable.WithColumn("AllergiesId").AsBinary().ForeignKey("FK_PatientAllergies_Allergies_AllergiesId", allergiesTableName, "Id").OnDelete(Rule.Cascade).Nullable();
                patientAllergiesTable.WithColumn("PatientsId").AsBinary().ForeignKey("FK_PatientAllergies_Patients_PatientsId", patientsTableName, "Id").OnDelete(Rule.Cascade).Nullable();
                patientAllergiesTable.WithColumn("Note").AsString().NotNullable();
                patientAllergiesTable.WithColumn("LastOcurrence").AsDateTime().NotNullable();
                patientAllergiesTable.WithColumn("AssertedDate").AsDateTime().NotNullable();

                this.Create.PrimaryKey("PK_PatientAllergies").OnTable(patientAllergiesTableName).Columns("IdAllergy", "IdPatient");
            }

            if (!this.Schema.Table(aspNetRolesTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetRolesTable = this.Create.Table(aspNetRolesTableName);
                aspNetRolesTable.WithColumn("Id").AsString().PrimaryKey().NotNullable();
                aspNetRolesTable.WithColumn("Name").AsString().NotNullable();
                aspNetRolesTable.WithColumn("NormalizedName").AsString().Nullable();
                aspNetRolesTable.WithColumn("ConcurrencyStamp").AsString().Nullable();
            }

            if (!this.Schema.Table(aspNetRoleClaimsTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetRoleClaimsTable = this.Create.Table(aspNetRoleClaimsTableName);
                aspNetRoleClaimsTable.WithColumn("Id").AsInt32().PrimaryKey().NotNullable();
                aspNetRoleClaimsTable.WithColumn("RoleId").AsString().NotNullable().ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", aspNetRolesTableName, "Id");
                aspNetRoleClaimsTable.WithColumn("ClaimType").AsString().Nullable();
                aspNetRoleClaimsTable.WithColumn("ClaimValue").AsString().Nullable();
            }

            if (!this.Schema.Table(aspNetUsersTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetUsersTable = this.Create.Table(aspNetUsersTableName);
                aspNetUsersTable.WithColumn("Id").AsString().PrimaryKey().NotNullable();
                aspNetUsersTable.WithColumn("UserName").AsString().Nullable();
                aspNetUsersTable.WithColumn("NormalizedUserName").AsString().Nullable();
                aspNetUsersTable.WithColumn("Email").AsString().Nullable();
                aspNetUsersTable.WithColumn("NormalizedEmail").AsString().Nullable();
                aspNetUsersTable.WithColumn("EmailConfirmed").AsInt32().NotNullable();
                aspNetUsersTable.WithColumn("PasswordHash").AsString().Nullable();
                aspNetUsersTable.WithColumn("SecurityStamp").AsString().Nullable();
                aspNetUsersTable.WithColumn("ConcurrencyStamp").AsString().Nullable();
                aspNetUsersTable.WithColumn("PhoneNumber").AsString().Nullable();
                aspNetUsersTable.WithColumn("PhoneNumberConfirmed").AsInt32().NotNullable();
                aspNetUsersTable.WithColumn("TwoFactorEnabled").AsInt32().NotNullable();
                aspNetUsersTable.WithColumn("LockoutEnd").AsString().Nullable();
                aspNetUsersTable.WithColumn("LockoutEnabled").AsInt32().NotNullable();
                aspNetUsersTable.WithColumn("AccessFailedCount").AsInt32().NotNullable();
                aspNetUsersTable.WithColumn("LastName").AsString().Nullable();
                aspNetUsersTable.WithColumn("Name").AsString().Nullable();
                aspNetUsersTable.WithColumn("RefreshToken").AsString().Nullable();
            }

            if (!this.Schema.Table(aspNetUsersClaimsTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetUserClaimsTable = this.Create.Table(aspNetUsersClaimsTableName);
                aspNetUserClaimsTable.WithColumn("Id").AsInt32().PrimaryKey().NotNullable();
                aspNetUserClaimsTable.WithColumn("UserId").AsString().ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", aspNetUsersTableName, "Id").OnDelete(Rule.Cascade).NotNullable();
                aspNetUserClaimsTable.WithColumn("ClaimType").AsString().Nullable();
                aspNetUserClaimsTable.WithColumn("ClaimValue").AsString().Nullable();
            }

            if (!this.Schema.Table(aspNetUserLoginsTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetUserLoginsTable = this.Create.Table(aspNetUserLoginsTableName);
                aspNetUserLoginsTable.WithColumn("LoginProvider").AsString().NotNullable();
                aspNetUserLoginsTable.WithColumn("ProviderKey").AsString().NotNullable();
                aspNetUserLoginsTable.WithColumn("ProviderDisplayName").AsString().Nullable();
                aspNetUserLoginsTable.WithColumn("UserId").AsString().NotNullable().ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", aspNetUsersTableName, "Id").OnDelete(Rule.Cascade);

                this.Create.PrimaryKey("PK_AspNetUserClaims").OnTable(aspNetUserLoginsTableName).Columns("LoginProvider", "ProviderKey");
            }

            if (!this.Schema.Table(aspNetUserRolesTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetUserRolesTable = this.Create.Table(aspNetUserRolesTableName);
                aspNetUserRolesTable.WithColumn("UserId").AsString().NotNullable().ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", aspNetUsersTableName, "Id").OnDelete(Rule.Cascade);
                aspNetUserRolesTable.WithColumn("RoleId").AsString().NotNullable().ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", aspNetRolesTableName, "Id").OnDelete(Rule.Cascade);

                this.Create.PrimaryKey("PK_AspNetUserRoles").OnTable(aspNetUserRolesTableName).Columns("UserId", "RoleId");
            }

            if (!this.Schema.Table(aspNetUserTokensTableName).Exists())
            {
                ICreateTableWithColumnOrSchemaOrDescriptionSyntax aspNetUserTokensTable = this.Create.Table(aspNetUserTokensTableName);
                aspNetUserTokensTable.WithColumn("UserId").AsString().ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", aspNetUsersTableName, "Id").NotNullable();
                aspNetUserTokensTable.WithColumn("LoginProvider").AsString().NotNullable();
                aspNetUserTokensTable.WithColumn("Name").AsString().NotNullable();
                aspNetUserTokensTable.WithColumn("Value").AsString().Nullable();

                this.Create.PrimaryKey("PK_AspNetUserTokens").OnTable(aspNetUserTokensTableName).Columns("UserId", "LoginProvider", "Name");
            }
        }
    }
}