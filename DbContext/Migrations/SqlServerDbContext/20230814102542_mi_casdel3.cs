using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_casdel3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AlterColumn<Guid>(
                name: "FriendId",
                schema: "supusr",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendId",
                principalSchema: "supusr",
                principalTable: "Friends",
                principalColumn: "FriendId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Friends_FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AlterColumn<Guid>(
                name: "FriendId",
                schema: "supusr",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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
    }
}
