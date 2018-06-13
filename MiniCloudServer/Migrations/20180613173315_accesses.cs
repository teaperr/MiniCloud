using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniCloudServer.Migrations
{
    public partial class accesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: false),
                    OwnerUserId = table.Column<int>(nullable: false),
                    DoneeUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceAccesses_Users_DoneeUserId",
                        column: x => x.DoneeUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceAccesses_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAccesses_DoneeUserId",
                table: "ResourceAccesses",
                column: "DoneeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAccesses_OwnerUserId",
                table: "ResourceAccesses",
                column: "OwnerUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceAccesses");
        }
    }
}
