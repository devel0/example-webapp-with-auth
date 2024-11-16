using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbmigrationspsql.Migrations
{
    /// <inheritdoc />
    public partial class removerotaterefreshtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rotated",
                table: "user_refresh_tokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "rotated",
                table: "user_refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
