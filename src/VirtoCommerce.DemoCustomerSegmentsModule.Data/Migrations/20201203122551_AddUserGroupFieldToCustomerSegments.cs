using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Migrations
{
    public partial class AddUserGroupFieldToCustomerSegments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserGroup",
                table: "DemoCustomerSegments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserGroup",
                table: "DemoCustomerSegments");
        }
    }
}
