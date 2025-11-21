using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class fakerdataindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "fake_datas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "fake_datas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "fake_datas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "fake_datas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_date_of_birth",
                table: "fake_datas",
                column: "date_of_birth");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_email",
                table: "fake_datas",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_first_name",
                table: "fake_datas",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_group_number",
                table: "fake_datas",
                column: "group_number");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_last_name",
                table: "fake_datas",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "IX_fake_datas_phone",
                table: "fake_datas",
                column: "phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_fake_datas_date_of_birth",
                table: "fake_datas");

            migrationBuilder.DropIndex(
                name: "IX_fake_datas_email",
                table: "fake_datas");

            migrationBuilder.DropIndex(
                name: "IX_fake_datas_first_name",
                table: "fake_datas");

            migrationBuilder.DropIndex(
                name: "IX_fake_datas_group_number",
                table: "fake_datas");

            migrationBuilder.DropIndex(
                name: "IX_fake_datas_last_name",
                table: "fake_datas");

            migrationBuilder.DropIndex(
                name: "IX_fake_datas_phone",
                table: "fake_datas");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "fake_datas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "fake_datas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "fake_datas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "fake_datas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
