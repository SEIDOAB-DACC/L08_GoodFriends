using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_anno4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "supusr",
                table: "Friends",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FirstName_LastName",
                schema: "supusr",
                table: "Friends",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_LastName_FirstName",
                schema: "supusr",
                table: "Friends",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetAddress_ZipCode_City_Country",
                schema: "supusr",
                table: "Addresses",
                columns: new[] { "StreetAddress", "ZipCode", "City", "Country" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friends_FirstName_LastName",
                schema: "supusr",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_LastName_FirstName",
                schema: "supusr",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StreetAddress_ZipCode_City_Country",
                schema: "supusr",
                table: "Addresses");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "supusr",
                table: "Friends",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)");
        }
    }
}
