using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FecebookAPI.Migrations
{
    public partial class AddUserFriend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UsersId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Posts",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UsersId",
                table: "Posts",
                newName: "IX_Posts_UserId1");

            migrationBuilder.AlterColumn<byte>(
                name: "image",
                table: "Posts",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserFriends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFriends_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_UserId",
                table: "UserFriends",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId1",
                table: "Posts",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId1",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "UserFriends");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Posts",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId1",
                table: "Posts",
                newName: "IX_Posts_UsersId");

            migrationBuilder.AlterColumn<byte>(
                name: "image",
                table: "Posts",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UsersId",
                table: "Posts",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
