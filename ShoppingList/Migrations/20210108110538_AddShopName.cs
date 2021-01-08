using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingList.Migrations
{
    public partial class AddShopName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                table: "ShoppingListEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopName",
                table: "ShoppingListEntities");
        }
    }
}
