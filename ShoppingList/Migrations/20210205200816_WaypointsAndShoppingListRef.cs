using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingList.Migrations
{
    public partial class WaypointsAndShoppingListRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShopName",
                table: "ShoppingListEntities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ShopWaypointsEntityId",
                table: "ShoppingListEntities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities",
                column: "ShopWaypointsEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities",
                column: "ShopWaypointsEntityId",
                principalTable: "ShopWaypointsEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListEntities_ShopWaypointsEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListEntities_ShopWaypointsEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.DropColumn(
                name: "ShopWaypointsEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.AlterColumn<string>(
                name: "ShopName",
                table: "ShoppingListEntities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
