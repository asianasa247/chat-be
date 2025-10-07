using ManageEmployee.Entities.ConfigurationEntities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class ConfigDataAriseBehaviour : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var listMigration = new List<ConfigAriseBehaviour>();

        // Mã nhập và ngày nhập => không theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalVoucherNumber",
            Name = "Mã",
            Code = "0001",
            Index = 1,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalBookDate",
            Name = "Ngày nhập",
            Code = "0002",
            Index = 2,
            Order = 1
        });

        // Phiếu nhập => không theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "referenceVoucherNumber",
            Name = "Phiếu nhập",
            Code = "0003",
            Index = 3,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "referenceBookDate",
            Name = "Ngày nhập phiếu",
            Code = "0004",
            Index = 4,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "referenceFullName",
            Name = "Phiếu nhập họ tên",
            Code = "0005",
            Index = 5,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "referenceAddress",
            Name = "Phiếu nhập địa chỉ",
            Code = "0006",
            Index = 6,
            Order = 1
        });

        // người nộp, địa chỉ => không theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalCompanyName",
            Name = "Người nộp",
            Code = "0007",
            Index = 7,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalAddress",
            Name = "Địa chỉ người nộp",
            Code = "0008",
            Index = 8,
            Order = 1
        });

        // diên giải, chứng từ kèm => không theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalDescription",
            Name = "Diễn giải",
            Code = "0009",
            Index = 9,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "attachVoucher",
            Name = "Chứng từ kèm",
            Code = "0010",
            Index = 10,
            Order = 1
        });

        // thông tin hóa đơn => co theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceCode",
            Name = "Loại H.Đơn",
            Code = "0011",
            Index = 11,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceNumber",
            Name = "Loại H.Đơn",
            Code = "0011.0001",
            Index = 1,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceSerial",
            Name = "Số seri",
            Code = "0011.0002",
            Index = 2,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceDate",
            Name = "Ngày H.Đơn",
            Code = "0011.0003",
            Index = 3,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceTaxCode",
            Name = "Mã số thuế",
            Code = "0011.0004",
            Index = 4,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceAddress",
            Name = "Địa chỉ",
            Code = "0011.0005",
            Index = 5,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceName",
            Name = "Đơn vị",
            Code = "0011.0006",
            Index = 6,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "invoiceProductItem",
            Name = "Sản phẩm",
            Code = "0011.0007",
            Index = 7,
            Order = 2
        });

        // tài khoản nợ => có theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "debitCode",
            Name = "TK Nợ",
            Code = "0012",
            Index = 12,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "debitDetailCodeFirst",
            Name = "C.tiết Nợ 1",
            Code = "0012.0001",
            Index = 1,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "debitDetailCodeSecond",
            Name = "C.tiết Nợ 2",
            Code = "0012.0002",
            Index = 2,
            Order = 2
        });

        // tài khoản có => theo cấp bậc
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "creditCode",
            Name = "TK Có",
            Code = "0013",
            Index = 13,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "creditDetailCodeFirst",
            Name = "C.tiết Có 1",
            Code = "0013.0001",
            Index = 1,
            Order = 2
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "creditDetailCodeSecond",
            Name = "C.tiết Có 2",
            Code = "0013.0002",
            Index = 2,
            Order = 2
        });

        // Mã kho
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "warehouse",
            Name = "Mã kho",
            Code = "0014",
            Index = 14,
            Order = 1
        });

        //Mã dự án => thanh tien
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "projectCode",
            Name = "Mã dự án",
            Code = "0015",
            Index = 15,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "depreciaMonth",
            Name = "Tháng khấu hao",
            Code = "0016",
            Index = 16,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "orginalCurrency",
            Name = "Ngoại tệ",
            Code = "0017",
            Index = 17,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "exchangeRate",
            Name = "Tỷ giá",
            Code = "0018",
            Index = 18,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "quantity",
            Name = "Số lượng",
            Code = "0019",
            Index = 19,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "unitPrice",
            Name = "Đơn giá",
            Code = "0020",
            Index = 20,
            Order = 1
        });
        listMigration.Add(new ConfigAriseBehaviour()
        {
            CodeData = "amount",
            Name = "Thành tiền",
            Code = "0021",
            Index = 21,
            Order = 1
        });
        var query = "delete from ConfigAriseDocumentBehaviour; delete from ConfigAriseBehaviour;";
        foreach (var item in listMigration)
        {
            query += $"insert into ConfigAriseBehaviour(CodeData, Name, Code, [Index], [Order]) values ('{item.CodeData}', N'{item.Name}', {item.Code}, {item.Index}, {item.Index});";
        }
        migrationBuilder.Sql(query);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
