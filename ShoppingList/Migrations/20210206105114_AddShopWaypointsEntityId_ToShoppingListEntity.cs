using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingList.Migrations
{
    public partial class AddShopWaypointsEntityId_ToShoppingListEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities",
                column: "ShopWaypointsEntityId",
                principalTable: "ShopWaypointsEntities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities",
                column: "ShopWaypointsEntityId",
                principalTable: "ShopWaypointsEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
