using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Migrations
{
    public partial class InitialCustomerSegments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemoCustomerSegments",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ExpressionTreeSerialized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoCustomerSegments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemoCustomerSegmentStore",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CustomerSegmentId = table.Column<string>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoCustomerSegmentStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoCustomerSegmentStore_DemoCustomerSegments_CustomerSegmentId",
                        column: x => x.CustomerSegmentId,
                        principalTable: "DemoCustomerSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemoCustomerSegmentStore_CustomerSegmentId",
                table: "DemoCustomerSegmentStore",
                column: "CustomerSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoCustomerSegmentStore_StoreId",
                table: "DemoCustomerSegmentStore",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemoCustomerSegmentStore");

            migrationBuilder.DropTable(
                name: "DemoCustomerSegments");
        }
    }
}
