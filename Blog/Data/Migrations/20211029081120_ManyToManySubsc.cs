using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Data.Migrations
{
    public partial class ManyToManySubsc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Blogs_BlogId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_BlogId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "Blogs");

            migrationBuilder.CreateTable(
                name: "BlogApplicationUser",
                columns: table => new
                {
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogApplicationUser", x => new { x.BlogId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_BlogApplicationUser_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogApplicationUser_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogApplicationUser_OwnerId",
                table: "BlogApplicationUser",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogApplicationUser");

            migrationBuilder.AddColumn<Guid>(
                name: "BlogId",
                table: "Blogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogId",
                table: "Blogs",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Blogs_BlogId",
                table: "Blogs",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
