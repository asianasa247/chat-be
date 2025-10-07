using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.ListCustomers;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ManageEmployee.Services.ListCustomerServices
{
    public class ListCustomerService : IListCustomerService
    {
        private readonly ApplicationDbContext _dbcontext;

        public ListCustomerService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<byte[]> GetCustomerContactHistories(int customerId)
        {
            //Lấy thông tin khách hàng
            var customer = await _dbcontext.Customers
                                           .Where(c => c.Id == customerId)
                                           .Select(c => new { c.Name })
                                           .FirstOrDefaultAsync();
            string customerName = customer != null ? customer.Name : "Không xác định";

            // Lấy dữ liệu từ CustomerContactHistories, Jobs, và Status
            var customerContacts = await (from cch in _dbcontext.CustomerContactHistories
                                          join job in _dbcontext.Jobs on cch.JobsId equals job.Id into jobGroup
                                          from job in jobGroup.DefaultIfEmpty()
                                          join st in _dbcontext.Status on cch.StatusId equals st.Id into statusGroup
                                          from st in statusGroup.DefaultIfEmpty()
                                          where cch.CustomerId == customerId
                                          select new
                                          {
                                              SourceType = "Liên hệ",
                                              ExchangeContent = cch.ExchangeContent,
                                              JobName = job != null ? job.Name : null,
                                              JobDescription = job != null ? job.Description : null,
                                              StatusName = st != null ? st.Name : null
                                          }).ToListAsync();

            // Lấy dữ liệu từ UserTask
            var userTasks = await _dbcontext.UserTasks
                                          .Where(ut => ut.CustomerId == customerId)
                                          .Select(ut => new
                                          {
                                              SourceType = "Công việc",
                                              UserTaskName = ut.Name,
                                              UserTaskDescription = ut.Description
                                          })
                                          .ToListAsync();


            //tạo file excel
            string fileMapServer = $"DanhSachCongViec_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\EXCEL");
            string pathSave = Path.Combine(folder, fileMapServer);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\DanhSachCongViec.xlsx");

            using (FileStream templateStream = System.IO.File.OpenRead(fullPath))
            using (ExcelPackage package = new ExcelPackage(templateStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                sheet.Cells[1, 1].Value = $"KHÁCH HÀNG: {customerName}";
                sheet.Cells[1, 1, 1, 6].Merge = true; //Gộp 6 cột
                sheet.Cells[1, 1].Style.Font.Size = 14;
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);

                int row = 4;
                int stt = 1;
                foreach (var contact in customerContacts)
                {
                    
                    sheet.Cells[row, 1].Value = stt++;
                    sheet.Cells[row, 2].Value = contact.SourceType;
                    sheet.Cells[row, 3].Value = contact.ExchangeContent;
                    sheet.Cells[row, 4].Value = contact.JobName;
                    sheet.Cells[row, 5].Value = contact.JobDescription;
                    sheet.Cells[row, 6].Value = contact.StatusName;
                    row++;
                    
                }
                row += 2;
                foreach (var task in userTasks)
                {
                    sheet.Cells[row, 1].Value = stt++;
                    sheet.Cells[row, 2].Value = task.SourceType;
                    sheet.Cells[row, 4].Value = task.UserTaskName;
                    sheet.Cells[row, 5].Value = task.UserTaskDescription;
                    row++;
                }

                // Đóng khung toàn bộ dữ liệu
                var range = sheet.Cells[3, 1, row - 1, 6];
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                sheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
    }

        

}

