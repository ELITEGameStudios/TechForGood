using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNiverse.Migrations
{
    /// <inheritdoc />
    public partial class AddColorsAndHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<float>(
                name: "Hours",
                table: "UserItems",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "UserItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "UserItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hours",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "UserItems");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
