using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class Add_Contractor_Tables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserToContractor",
            columns: table => new
            {
                user_to_contractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                userId = table.Column<int>(type: "int", nullable: false),
                domain = table.Column<string>(type: "nvarchar(450)", nullable: true),
                isDeleted = table.Column<bool>(type: "bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserToContractor", x => x.user_to_contractorId);
                table.ForeignKey(
                    name: "FK_UserToContractor_Users_userId",
                    column: x => x.userId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ContractorToCategory",
            columns: table => new
            {
                contractor_to_categoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                user_to_contractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                category_name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                sort_order = table.Column<int>(type: "int", nullable: false),
                isDeleted = table.Column<bool>(type: "bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContractorToCategory", x => x.contractor_to_categoryId);
                table.ForeignKey(
                    name: "FK_ContractorToCategory_UserToContractor_contractor_to_categoryId",
                    column: x => x.contractor_to_categoryId,
                    principalTable: "UserToContractor",
                    principalColumn: "user_to_contractorId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ContractorToCategoryToProduct",
            columns: table => new
            {
                contractor_to_category_to_productId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ContractToCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                productId = table.Column<int>(type: "int", nullable: false),
                ContractorToCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContractorToCategoryToProduct", x => x.contractor_to_category_to_productId);
                table.ForeignKey(
                    name: "FK_ContractorToCategoryToProduct_ContractorToCategory_ContractorToCategoryId",
                    column: x => x.ContractorToCategoryId,
                    principalTable: "ContractorToCategory",
                    principalColumn: "contractor_to_categoryId");
                table.ForeignKey(
                    name: "FK_ContractorToCategoryToProduct_ContractorToCategory_ContractToCategoryId",
                    column: x => x.ContractToCategoryId,
                    principalTable: "ContractorToCategory",
                    principalColumn: "contractor_to_categoryId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ContractorToCategory_category_name_user_to_contractorId_sort_order",
            table: "ContractorToCategory",
            columns: new[] { "category_name", "user_to_contractorId", "sort_order" });

        migrationBuilder.CreateIndex(
            name: "IX_ContractorToCategoryToProduct_ContractorToCategoryId",
            table: "ContractorToCategoryToProduct",
            column: "ContractorToCategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ContractorToCategoryToProduct_ContractToCategoryId_productId",
            table: "ContractorToCategoryToProduct",
            columns: new[] { "ContractToCategoryId", "productId" });

        migrationBuilder.CreateIndex(
            name: "IX_UserToContractor_userId_domain",
            table: "UserToContractor",
            columns: new[] { "userId", "domain" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ContractorToCategoryToProduct");

        migrationBuilder.DropTable(
            name: "ContractorToCategory");

        migrationBuilder.DropTable(
            name: "UserToContractor");
    }
}
