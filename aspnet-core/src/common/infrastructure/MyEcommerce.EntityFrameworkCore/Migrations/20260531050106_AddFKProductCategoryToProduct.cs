using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddFKProductCategoryToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_CategoryId",
                table: "AppProducts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppProductCategories_CategoryId",
                table: "AppProducts",
                column: "CategoryId",
                principalTable: "AppProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppProductCategories_CategoryId",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_CategoryId",
                table: "AppProducts");
        }
    }
}
