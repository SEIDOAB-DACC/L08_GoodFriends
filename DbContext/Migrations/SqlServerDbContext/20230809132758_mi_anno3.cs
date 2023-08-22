using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class mi_anno3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "supusr");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Quotes",
                newName: "Quotes",
                newSchema: "supusr");

            migrationBuilder.RenameTable(
                name: "Pets",
                newName: "Pets",
                newSchema: "supusr");

            migrationBuilder.RenameTable(
                name: "Friends",
                newName: "Friends",
                newSchema: "supusr");

            migrationBuilder.RenameTable(
                name: "csFriendDbMcsQuoteDbM",
                newName: "csFriendDbMcsQuoteDbM",
                newSchema: "supusr");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Addresses",
                newSchema: "supusr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "dbo",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Quotes",
                schema: "supusr",
                newName: "Quotes");

            migrationBuilder.RenameTable(
                name: "Pets",
                schema: "supusr",
                newName: "Pets");

            migrationBuilder.RenameTable(
                name: "Friends",
                schema: "supusr",
                newName: "Friends");

            migrationBuilder.RenameTable(
                name: "csFriendDbMcsQuoteDbM",
                schema: "supusr",
                newName: "csFriendDbMcsQuoteDbM");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "supusr",
                newName: "Addresses");
        }
    }
}
