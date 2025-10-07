using Emgu.CV;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Services.Interfaces.NewHotels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.NewHotelServices
{
    public class AreaService : IAreaService
    {
        private readonly ApplicationDbContext dbContext;

        public AreaService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Area> Add(AreaDTO model)
        {
            var area = new Area
            {
                Code = model.Code,
                Name = model.Name
            };

            await dbContext.AddAsync(area);
            await dbContext.SaveChangesAsync();
            return area;
        }

        public async Task<Area> Edit(int Id, AreaDTO model)
        {
            var area = await dbContext.Areas.FirstOrDefaultAsync(a => a.Id == Id);
            if (area == null) return null;

            area.Code = model.Code;
            area.Name = model.Name;

            dbContext.Update(area);
            await dbContext.SaveChangesAsync();
            return area;
        }

        public async Task<bool> Delete(int Id)
        {
            var area = await dbContext.Areas.FirstOrDefaultAsync(a => a.Id == Id);
            if (area == null) return false;

            dbContext.Remove(area);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Area>> GetAll()
        {
            return await dbContext.Areas.ToListAsync();
        }

        public async Task<Area> GetById(int Id)
        {
            return await dbContext.Areas.FirstOrDefaultAsync(a => a.Id == Id);
        }

        public async Task<object> GetPaged(PagingRequestModel pagingRequest)
        {
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return null;
            }

            var query = dbContext.Areas.AsQueryable();

            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(x => x.Name.Contains(pagingRequest.SearchText) || x.Code.Contains(pagingRequest.SearchText));
            }

            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "code" => query.OrderBy(x => x.Code),
                    _ => query.OrderBy(x => x.Id)
                };
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pagingRequest.PageSize);
            var areas = await query.Skip((pagingRequest.Page - 1) * pagingRequest.PageSize).Take(pagingRequest.PageSize).ToListAsync();

            return new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pagingRequest.Page,
                PageSize = pagingRequest.PageSize,
                Data = areas
            };
        }

        public async Task<string> ExportExcel()
        {
            string fileMapServer = $"DanhSachKhuVuc_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                   folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                   pathSave = Path.Combine(folder, fileMapServer);
            var data = await dbContext.Areas.ToListAsync();
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\DanhSachKhuVuc.xlsx");
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(fullPath))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 3;
                    int nCol = 4;
                    int rowIdx = nRowBegin;
                    var rows = data;

                    if (rows.Count > 0)
                    {
                        int i = 0;
                        foreach (var row in rows)
                        {
                            i++;
                            sheet.Cells[rowIdx, 1].Value = i.ToString();
                            sheet.Cells[rowIdx, 2].Value = row.Id;
                            sheet.Cells[rowIdx, 3].Value = row.Code;
                            sheet.Cells[rowIdx, 4].Value = row.Name;

                            var range = sheet.Cells[rowIdx, 1, rowIdx, nCol];
                            range.Style.Font.Name = "Arial"; // Đổi thành font bạn muốn
                            range.Style.Font.Size = 12; // Kích thước chữ
                            range.Style.Font.Bold = false; // Không in đậm
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left; // Canh lề trái
                            rowIdx++;

                        }
                    }

                    //Thiết lập viền bảng cho toàn bộ vùng dữ liệu
                    
                    if (data.Any())
                    {
                        var range = sheet.Cells[3, 1, rowIdx - 1, nCol];
                        
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    //TỰ ĐỘNG CANH CHỈNH ĐỘ RỘNG CỘT
                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    using (FileStream fs = new FileStream(pathSave, FileMode.Create))
                    {
                        await package.SaveAsAsync(fs);
                    }
                }
            }
            return fileMapServer;

        }

        public async Task<string> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return "File không hợp lệ!";
            }

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Excel");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = Path.Combine(folder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var areasToUpdate = new List<Area>();
            var areasToInsert = new List<Area>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.FirstOrDefault();
                if (sheet == null)
                {
                    return "Không tìm thấy sheet trong file Excel!";
                }

                int rowCount = sheet.Dimension.Rows;
                var existingIds = await dbContext.Areas.Select(a => a.Id).ToListAsync();

                for (int row = 3; row <= rowCount; row++) // Bỏ qua tiêu đề
                {
                    var idValue = sheet.Cells[row, 2].Value?.ToString();
                    var code = sheet.Cells[row, 3].Value?.ToString();
                    var name = sheet.Cells[row, 4].Value?.ToString();

                    if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
                    {
                        if (!string.IsNullOrEmpty(idValue) && int.TryParse(idValue, out int id) && existingIds.Contains(id))
                        {
                            // Nếu ID tồn tại, cập nhật dữ liệu
                            var existingArea = await dbContext.Areas.FindAsync(id);
                            if (existingArea != null)
                            {
                                existingArea.Code = code;
                                existingArea.Name = name;
                                areasToUpdate.Add(existingArea);
                            }
                        }
                        else
                        {
                            // Nếu ID không tồn tại, thêm mới
                            areasToInsert.Add(new Area
                            {
                                Code = code,
                                Name = name
                            });
                        }
                    }
                }
            }

            try
            {
                if (areasToUpdate.Any())
                {
                    dbContext.Areas.UpdateRange(areasToUpdate);
                }
                if (areasToInsert.Any())
                {
                    await dbContext.Areas.AddRangeAsync(areasToInsert);
                }

                await dbContext.SaveChangesAsync();
                return $"Cập nhật {areasToUpdate.Count} khu vực, thêm mới {areasToInsert.Count} khu vực!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}";
            }
        }


    }
}
