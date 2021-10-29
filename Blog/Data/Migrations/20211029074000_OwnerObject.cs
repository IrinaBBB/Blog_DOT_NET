using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Data.Migrations
{
    public partial class OwnerObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OwnerId1",
                table: "Posts",
                column: "OwnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OwnerId1",
                table: "Comments",
                column: "OwnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_OwnerId1",
                table: "Blogs",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_OwnerId1",
                table: "Blogs",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_OwnerId1",
                table: "Comments",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId1",
                table: "Posts",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_OwnerId1",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_OwnerId1",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_OwnerId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_OwnerId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_OwnerId1",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Blogs");
        }
    }
}
