using Microsoft.EntityFrameworkCore.Migrations;

namespace Blogger.Infrastructure.Migrations
{
    public partial class UpdateAuthorEntityIdToStringFromInteger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId1",
                table: "Comments",
                column: "AuthorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId1",
                table: "Comments",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
