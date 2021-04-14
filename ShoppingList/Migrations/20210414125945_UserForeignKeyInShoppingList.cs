using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingList.Migrations
{
    public partial class UserForeignKeyInShoppingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "ShoppingListEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListEntities_UserEntityId",
                table: "ShoppingListEntities",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListEntities_UserEntities_UserEntityId",
                table: "ShoppingListEntities",
                column: "UserEntityId",
                principalTable: "UserEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListEntities_UserEntities_UserEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListEntities_UserEntityId",
                table: "ShoppingListEntities");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "ShoppingListEntities");
        }
    }
}
