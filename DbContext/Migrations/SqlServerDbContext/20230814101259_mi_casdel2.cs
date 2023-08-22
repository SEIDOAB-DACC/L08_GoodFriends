using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_casdel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendId",
                principalSchema: "supusr",
                principalTable: "Friends",
                principalColumn: "FriendId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendId",
                principalSchema: "supusr",
                principalTable: "Friends",
                principalColumn: "FriendId");
        }
    }
}
