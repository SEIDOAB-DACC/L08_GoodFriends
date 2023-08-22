using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_anno5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Addresses_AddressDbMAddressId",
                schema: "supusr",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "AddressDbMAddressId",
                schema: "supusr",
                table: "Friends",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_AddressDbMAddressId",
                schema: "supusr",
                table: "Friends",
                newName: "IX_Friends_AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Addresses_AddressId",
                schema: "supusr",
                table: "Friends",
                column: "AddressId",
                principalSchema: "supusr",
                principalTable: "Addresses",
                principalColumn: "AddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Addresses_AddressId",
                schema: "supusr",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "supusr",
                table: "Friends",
                newName: "AddressDbMAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_AddressId",
                schema: "supusr",
                table: "Friends",
                newName: "IX_Friends_AddressDbMAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Addresses_AddressDbMAddressId",
                schema: "supusr",
                table: "Friends",
                column: "AddressDbMAddressId",
                principalSchema: "supusr",
                principalTable: "Addresses",
                principalColumn: "AddressId");
        }
    }
}
