using Microsoft.EntityFrameworkCore.Migrations;

namespace CH.Project.Migrations
{
    public partial class _010504_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderDetail");
        }
    }
}
