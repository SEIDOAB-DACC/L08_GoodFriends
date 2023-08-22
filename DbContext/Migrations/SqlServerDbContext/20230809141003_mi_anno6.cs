using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_anno6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Friends_FriendDbMFriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_FriendDbMFriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "FriendDbMFriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AddColumn<Guid>(
                name: "FriendId",
                schema: "supusr",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Pets_FriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendId");

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

            migrationBuilder.DropIndex(
                name: "IX_Pets_FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "FriendId",
                schema: "supusr",
                table: "Pets");

            migrationBuilder.AddColumn<Guid>(
                name: "FriendDbMFriendId",
                schema: "supusr",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_FriendDbMFriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendDbMFriendId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Friends_FriendDbMFriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendDbMFriendId",
                principalSchema: "supusr",
                principalTable: "Friends",
                principalColumn: "FriendId");
        }
    }
}
