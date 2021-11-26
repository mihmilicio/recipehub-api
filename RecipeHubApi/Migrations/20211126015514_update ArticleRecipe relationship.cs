using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeHubApi.Migrations
{
    public partial class updateArticleReciperelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleRecipe");

            migrationBuilder.AddColumn<string>(
                name: "ArticleId",
                table: "Recipe",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_ArticleId",
                table: "Recipe",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Article_ArticleId",
                table: "Recipe",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Article_ArticleId",
                table: "Recipe");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_ArticleId",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Recipe");

            migrationBuilder.CreateTable(
                name: "ArticleRecipe",
                columns: table => new
                {
                    ArticlesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecipesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleRecipe", x => new {x.ArticlesId, x.RecipesId});
                    table.ForeignKey(
                        name: "FK_ArticleRecipe_Article_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleRecipe_Recipe_RecipesId",
                        column: x => x.RecipesId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleRecipe_RecipesId",
                table: "ArticleRecipe",
                column: "RecipesId");
        }
    }
}