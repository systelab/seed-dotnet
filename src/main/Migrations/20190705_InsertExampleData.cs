namespace main.Migrations
{
    using System.Collections.Generic;
    using System.Data;
    using FluentMigrator;
    using FluentMigrator.Builders.Create.Table;

    /// <summary>
    /// Insertion of example data into different tables.
    /// </summary>
    [Migration(20190705141100)]
    public class InsertExampleData : Migration
    {
        public override void Up()
        {
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

            if (this.Schema.Table(aspNetUsersTableName).Exists())
            {
                List<Dictionary<string, object>> usersData = new List<Dictionary<string, object>>();

                Dictionary<string, object> dictionary = new Dictionary<string, object>
                {
                    {"Id", "f722a56c-4994-44c2-8016-ab891236a3fc"},
                    {"UserName", "Systelab"},
                    {"NormalizedUserName", "SYSTELAB"},
                    {"Email", "Systelab@werfen.com"},
                    {"NormalizedEmail", "SYSTELAB@WERFEN.COM"},
                    {"EmailConfirmed", 1},
                    {
                        "PasswordHash",
                        "AQAAAAEAACcQAAAAEG9uV4eOcSTSn1aYq3pqDsf53Qh4iUASzCDUMlK73MlP30KbQ8Q7gfcfonzSBQdFLg=="
                    },
                    {"SecurityStamp", "OWB4UGO65HXX4O6NY4YOQEAGRHZ5XXZ4"},
                    {"ConcurrencyStamp", "420e125e-f656-40ce-ac0a-50ecd8941f2d"},
                    {"PhoneNumber", null},
                    {"PhoneNumberConfirmed", 0},
                    {"TwoFactorEnabled", 0},
                    {"LockoutEnd", null},
                    {"LockoutEnabled", 1},
                    {"AccessFailedCount", 0},
                    {"LastName", "Seed_Dotnet"},
                    {"Name", "Systelab"},
                    {
                        "RefreshToken",
                        "AQAAAAEAACcQAAAAEFJL9inwjs3L8babcXfB0Ll1EmGXSh3BAPnqpmPZongRXApS4q1TJPeRTzR9AA"
                    }
                };

                usersData.Add(dictionary);

                dictionary = new Dictionary<string, object>
                {
                    {"Id", "490782fd-5ad7-4a5d-9730-bdec562b3d2a"},
                    {"UserName", "admin"},
                    {"NormalizedUserName", "ADMIN"},
                    {"Email", "admin@werfen.com"},
                    {"NormalizedEmail", "ADMIN@WERFEN.COM"},
                    {"EmailConfirmed", 1},
                    {
                        "PasswordHash",
                        "AQAAAAEAACcQAAAAEOhlde9z5PlVn2NfkQGKLyQvMiqjjyArLzTHePUnlIGkqfImIBb9P4HWZmu/MAvmBg=="
                    },
                    {"SecurityStamp", "F3WXITGHUY36Q5ZGIGD2QIJNEBGOZI6V"},
                    {"ConcurrencyStamp", "cf92e5b8-3f51-4621-9918-c0e6e684e7c4"},
                    {"PhoneNumber", null},
                    {"PhoneNumberConfirmed", 0},
                    {"TwoFactorEnabled", 0},
                    {"LockoutEnd", null},
                    {"LockoutEnabled", 1},
                    {"AccessFailedCount", 0},
                    {"LastName", "Seed_Dotnet"},
                    {"Name", "Administrator"},
                    {
                        "RefreshToken",
                        null
                    }
                };
                usersData.Add(dictionary);

                dictionary = new Dictionary<string, object>
                {
                    {"Id", "071fe4b2-8759-44e1-be2f-97ffc0efe03f"},
                    {"UserName", "quentinada"},
                    {"NormalizedUserName", "QUENTINADA"},
                    {"Email", "quentinada@werfen.com"},
                    {"NormalizedEmail", "QUENTINADA@WERFEN.COM"},
                    {"EmailConfirmed", 1},
                    {
                        "PasswordHash",
                        "AQAAAAEAACcQAAAAEH1aDFb5oust8rwEywu79VzxlWFRdKtSZjPq8TGU23I9cKUpC0kzexoR00QO8tHROg=="
                    },
                    {"SecurityStamp", "T6D7IIZTOGPB6HLAEOOHN3VJXK7WFEC7"},
                    {"ConcurrencyStamp", "a31cea8c-3141-4bfb-b86a-002fb595b24b"},
                    {"PhoneNumber", null},
                    {"PhoneNumberConfirmed", 0},
                    {"TwoFactorEnabled", 0},
                    {"LockoutEnd", null},
                    {"LockoutEnabled", 1},
                    {"AccessFailedCount", 0},
                    {"LastName", "quentinada"},
                    {"Name", "quentinada"},
                    {
                        "RefreshToken",
                        null
                    }
                };

                usersData.Add(dictionary);

                dictionary = new Dictionary<string, object>
                {
                    {"Id", "421399dc-94eb-4749-be82-898b70c3edb7"},
                    {"UserName", "test"},
                    {"NormalizedUserName", "TEST"},
                    {"Email", "testadmin@werfen.com"},
                    {"NormalizedEmail", "TESTADMIN@WERFEN.COM"},
                    {"EmailConfirmed", 1},
                    {
                        "PasswordHash",
                        "AQAAAAEAACcQAAAAENJwVqkRgnyD6+wjh3JANHvmA1yug+Sn0T6eITF+rpt9jtmxinXcADIxNjBNC6v8gw=="
                    },
                    {"SecurityStamp", "RKZK6OSCA43FXHQVX7YVYGY7KAKTG6FU"},
                    {"ConcurrencyStamp", "efd1047c-bc44-48de-bdc3-06e7bdbc1aae"},
                    {"PhoneNumber", null},
                    {"PhoneNumberConfirmed", 0},
                    {"TwoFactorEnabled", 0},
                    {"LockoutEnd", null},
                    {"LockoutEnabled", 1},
                    {"AccessFailedCount", 0},
                    {"LastName", "test_Seed_Dotnet"},
                    {"Name", "test"},
                    {
                        "RefreshToken",
                        null
                    }
                };

                usersData.Add(dictionary);

                foreach (Dictionary<string, object> user in usersData)
                {
                    this.Insert.IntoTable(aspNetUsersTableName).Row(user);
                }
            }

        }
        public override void Down()
        {
        }
    }
}
