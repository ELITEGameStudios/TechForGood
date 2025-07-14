using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNiverse.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoadouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquippedItems",
                table: "UserItems");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_FaceItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_HeadItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_PantsItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_PetItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_ShirtItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Loadout_ShoesItemId",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Loadout_FaceItemId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "Loadout_HeadItemId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "Loadout_PantsItemId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "Loadout_PetItemId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "Loadout_ShirtItemId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "Loadout_ShoesItemId",
                table: "UserItems");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "EquippedItems",
                table: "UserItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
