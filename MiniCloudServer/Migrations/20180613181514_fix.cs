using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniCloudServer.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceAccesses_OwnerUserId",
                table: "ResourceAccesses");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAccesses_OwnerUserId",
                table: "ResourceAccesses",
                column: "OwnerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceAccesses_OwnerUserId",
                table: "ResourceAccesses");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAccesses_OwnerUserId",
                table: "ResourceAccesses",
                column: "OwnerUserId",
                unique: true);
        }
    }
}
