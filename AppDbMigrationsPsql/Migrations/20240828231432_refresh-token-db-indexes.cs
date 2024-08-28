using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDbMigrationsPsql.Migrations
{
    /// <inheritdoc />
    public partial class refreshtokendbindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_RefreshToken",
                table: "UserRefreshTokens",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserName_RefreshToken",
                table: "UserRefreshTokens",
                columns: new[] { "UserName", "RefreshToken" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_RefreshToken",
                table: "UserRefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_UserName_RefreshToken",
                table: "UserRefreshTokens");
        }
    }
}
