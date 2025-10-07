using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTblVintaxInvoiceIns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VintaxInvoiceIns",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nbten = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nbdchi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shdon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nbmst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    khhdon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    khmshdon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nmten = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nmmst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nmdchi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tgtttbso = table.Column<double>(type: "float", nullable: false),
                    ntao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VintaxInvoiceIns", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VintaxInvoiceIns");
        }
    }
}
