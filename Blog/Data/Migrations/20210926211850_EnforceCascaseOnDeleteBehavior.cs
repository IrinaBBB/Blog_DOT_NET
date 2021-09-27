using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Data.Migrations
{
    public partial class EnforceCascaseOnDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Blogs_BlogId1",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Blogs_BlogId1",
                table: "Posts",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Blogs_BlogId1",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Blogs_BlogId1",
                table: "Posts",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
