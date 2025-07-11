using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNiverse.Migrations
{
    /// <inheritdoc />
    public partial class AddEquips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EquippedItems",
                table: "UserItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cosmetics",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquippedItems",
                table: "UserItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cosmetics",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
