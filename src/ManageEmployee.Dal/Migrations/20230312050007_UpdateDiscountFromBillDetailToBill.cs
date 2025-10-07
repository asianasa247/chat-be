using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateDiscountFromBillDetailToBill : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("delete Bills where Id not in (select BillId from BillDetails)");
        migrationBuilder.Sql(@"update Bills
  set DiscountPrice = (select sum(DiscountPrice) from (select BillId,  case when DiscountType = 'money' then sum(DiscountPrice * Quality)
					   when DiscountType = 'percent' then sum(DiscountPrice * Quality * UnitPrice/ 100)
					   end as DiscountPrice from BillDetails 
					   group by BillId, DiscountType) as BillDetails
					    where Bills.Id = BillDetails.BillId )
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
