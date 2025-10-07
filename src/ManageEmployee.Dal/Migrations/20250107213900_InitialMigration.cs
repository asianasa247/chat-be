using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "ConfigurationViews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "[\n                        { label: 'Tiền mặt', value: 'TM' },\n                                { label: 'Công nợ', value: 'CN' },\n                                { label: 'Ngân hàng', value: 'NH' },\n                    ]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConfigurationViews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "[\r\n                        { label: 'Tiền mặt', value: 'TM' },\r\n                                { label: 'Công nợ', value: 'CN' },\r\n                                { label: 'Ngân hàng', value: 'NH' },\r\n                    ]");
        }
    }
}
