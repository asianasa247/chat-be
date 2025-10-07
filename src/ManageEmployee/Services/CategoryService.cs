using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.Json;
using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Categories;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services;
public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategorySelectionModel>> GetAll()
    {
        var data = await _context.Categories.Where(x => !x.IsDeleted)
                        .Select(X => new CategorySelectionModel
                        {
                            Id = X.Id,
                            Code = X.Code,
                            Name = X.Name,
                            CodeName = X.Code + " - " + X.Name,
                            Type = X.Type,
                            TypeView = X.TypeView,
                            IsSizeImage = X.IsSizeImage,
                            TypeMenu = X.TypeMenu,
                        }).ToListAsync();
        return data;
    }

    public async Task<IEnumerable<CategorySelectionModel>> GetAll(List<int> types)
    {
        var data = await _context.Categories.Where(x => !x.IsDeleted && types.Contains(x.Type))
                        .Select(X => new CategorySelectionModel
                        {
                            Id = X.Id,
                            Code = X.Code,
                            Name = X.Name,
                            CodeName = X.Code + " - " + X.Name,
                            Type = X.Type,
                            TypeView = X.TypeView,
                            IsSizeImage = X.IsSizeImage,
                            TypeMenu = X.TypeMenu,
                        }).ToListAsync();
        return data;
    }

    public async Task<PagingResult<Category>> GetAll(int pageIndex, int pageSize, string keyword, int? type)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            if (type == null || type == 0)
            {
                var categories = (from p in _context.Categories
                                  where (keyword != null && keyword.Length > 0 ? (
                                     p.Name.Trim().Contains(keyword) || p.Name.Trim().StartsWith(keyword) || p.Name.Trim().EndsWith(keyword) ||
                                     p.Note.Trim().Contains(keyword) || p.Note.Trim().StartsWith(keyword) || p.Note.Trim().EndsWith(keyword)
                                     ) : p.Id != 0) && !p.IsDeleted
                                  select new Category
                                  {
                                      Id = p.Id,
                                      Code = p.Code,
                                      Name = p.Name,
                                      Note = p.Note,
                                      Type = p.Type,
                                      IsEnableDelete = p.IsEnableDelete,
                                      IsShowWeb = p.IsShowWeb,
                                      IsSizeImage = p.IsSizeImage,
                                      TypeMenu = p.TypeMenu,
                                      TotalAmountBuy = p.TotalAmountBuy,
                                      NumberItem = p.NumberItem,
                                      ProductCount = p.ProductCount   // NEW
                                  });
                return new PagingResult<Category>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    TotalItems = await categories.CountAsync(),
                    Data = await categories.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
                };
            }
            else
            {
                var categories = (from p in _context.Categories
                                  where (keyword != null && keyword.Length > 0 ? (
                                     p.Name.Trim().Contains(keyword) || p.Name.Trim().StartsWith(keyword) || p.Name.Trim().EndsWith(keyword) ||
                                     p.Note.Trim().Contains(keyword) || p.Note.Trim().StartsWith(keyword) || p.Note.Trim().EndsWith(keyword)
                                     ) : p.Id != 0) && !p.IsDeleted && p.Type == type
                                  select new Category
                                  {
                                      Id = p.Id,
                                      Code = p.Code,
                                      Name = p.Name,
                                      Note = p.Note,
                                      Type = p.Type,
                                      IsPublish = p.IsPublish,
                                      Icon = p.Icon,
                                      Image = p.Image,
                                      CodeParent = p.CodeParent,
                                      NumberItem = p.NumberItem,
                                      ProductCount = p.ProductCount,   // NEW
                                      NameKorea = p.NameKorea,
                                      NameEnglish = p.NameEnglish,
                                      IsEnableDelete = p.IsEnableDelete,
                                      IsShowWeb = p.IsShowWeb,
                                      IsSizeImage = p.IsSizeImage,
                                      TypeMenu = p.TypeMenu,
                                      TotalAmountBuy = p.TotalAmountBuy
                                  });
                return new PagingResult<Category>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    TotalItems = await categories.CountAsync(),
                    Data = await categories.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
                };
            }
        }
        catch
        {
            return new PagingResult<Category>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<Category>()
            };
        }
    }

    public async Task<CategoryModel> GetById(int id)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            var itemOut = _mapper.Map<CategoryModel>(category);
            itemOut.FileLink = category.Image != null
                && category.Image != "" ? JsonSerializer.Deserialize<List<UserTaskFileModel>>(category.Image) : new List<UserTaskFileModel>();
            // ProductCount đã map qua AutoMapper; không cần set tay.
            return itemOut;
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Create(CategoryModel param)
    {
        try
        {
            var exist = await _context.Categories.SingleOrDefaultAsync(
                x => x.Code == param.Code && x.Type == param.Type && !x.IsDeleted);
            if (exist != null)
            {
                return ErrorMessages.CategoryNameAlreadyExist;
            }

            var category = _mapper.Map<Category>(param);
            category.Image = param.FileLink != null && param.FileLink.Count > 0 ? JsonSerializer.Serialize(param.FileLink) : "";
            category.ProductCount = param.ProductCount; // NEW

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return string.Empty;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task<string> Update(CategoryModel param)
    {
        try
        {
            var category = await _context.Categories.SingleOrDefaultAsync(x => x.Id == param.Id && !x.IsDeleted);
            if (category == null)
            {
                return ErrorMessages.DataNotFound;
            }
            var exist = await _context.Categories.SingleOrDefaultAsync(
                x => x.Code == param.Code && x.Type == param.Type && !x.IsDeleted && x.Id != param.Id);
            if (exist != null)
            {
                return ErrorMessages.CategoryNameAlreadyExist;
            }

            if (param.Type == (int)CategoryEnum.MenuWeb)
            {
                category.IsPublish = param.IsPublish;
                category.Icon = param.Icon;
                category.CodeParent = param.CodeParent;
            }
            if (param.Type == (int)CategoryEnum.MenuWebOnePage)
            {
                category.IsPublish = param.IsPublish;
                category.Icon = param.Icon;
                category.CodeParent = param.CodeParent;
            }

            category.NumberItem = param.NumberItem;
            category.ProductCount = param.ProductCount; // NEW

            category.Code = param.Code;
            category.Name = param.Name;
            category.Note = param.Note;
            category.Type = param.Type;
            category.NameKorea = param.NameKorea;
            category.NameEnglish = param.NameEnglish;
            category.IsShowWeb = param.IsShowWeb;
            category.TypeView = param.TypeView;
            category.TotalAmountBuy = param.TotalAmountBuy;
            category.Image = param.FileLink != null && param.FileLink.Count > 0 ? JsonSerializer.Serialize(param.FileLink) : "";
            category.IsSizeImage = param.IsSizeImage;
            category.TypeMenu = param.TypeMenu;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category.Type == (int)CategoryEnum.PriceList && await CheckExistInGoodsAsync(category.Code))
        {
            throw new ErrorException(ResultErrorConstants.PRICE_LIST_EXISTING_IN_GOODS);
        }
        category.IsDeleted = true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WebCategoryViewModel>> GetCategoryForWeb()
    {
        try
        {
            var goods = await _context.Goods.ToListAsync();

            var menus = await _context.Categories.Where(x => x.Type == (int)CategoryEnum.MenuWeb && !x.IsDeleted && x.IsShowWeb).ToListAsync();

            var parentMenus = menus.Where(x => string.IsNullOrEmpty(x.CodeParent)).ToList();
            var childMenus = menus.Where(x => !string.IsNullOrEmpty(x.CodeParent)).ToList();
            var result = new List<WebCategoryViewModel>();
            foreach (var menu in parentMenus)
            {
                var category = new WebCategoryViewModel
                {
                    Code = menu.Code,
                    Name = menu.Name,
                    NameEnglish = menu.NameEnglish,
                    NameKorea = menu.NameKorea,
                    Image = menu.Image,
                    Icon = menu.Icon,
                    IsSizeImage = menu.IsSizeImage,
                    TypeMenu = menu.TypeMenu
                };

                await GetChildMenusByCategory(category, goods, childMenus);

                result.Add(category);
            }
            return result;
        }
        catch
        {
            return new List<WebCategoryViewModel>();
        }
    }

    private async Task GetChildMenusByCategory(WebCategoryViewModel parent, List<Goods> goods, List<Category> childMenus)
    {
        var menus = childMenus.Where(x => x.CodeParent == parent.Code).ToList();

        foreach (var menu in menus)
        {
            var child = new WebCategoryViewModel
            {
                Code = menu.Code,
                Name = menu.Name,
                NameEnglish = menu.NameEnglish,
                NameKorea = menu.NameKorea,
                Image = menu.Image,
                Icon = menu.Icon,
                IsSizeImage = menu.IsSizeImage,
                TypeMenu = menu.TypeMenu
            };

            // Children loop
            await GetChildMenusByCategory(child, goods, childMenus);
            await AddChildMenu(child, goods);
            parent.Childrens.Add(child);
        }
    }

    async Task AddChildMenu(WebCategoryViewModel parent, List<Goods> goods)
    {
        var categoryWebId = await _context.CategoryStatusWebPeriods.Where(x => x.CategoryId == parent.Id).Select(x => x.Id).FirstOrDefaultAsync();
        var goodIds = await _context.CategoryStatusWebPeriodGoods.Where(x => x.CategoryStatusWebPeriodId == categoryWebId).Select(x => x.GoodId).ToListAsync();
        var chidGoods = goods.Where(g => goodIds.Contains(g.Id)).ToList();
        foreach (var good in chidGoods)
        {
            WebCategoryViewModel category_goodChil = new()
            {
                Code = !string.IsNullOrEmpty(good.Detail2) ? good.Detail2 : good.Detail1,
                Name = good.TitleVietNam,
                NameEnglish = good.TitleEnglish,
                NameKorea = good.TitleKorea
            };

            parent.Childrens.Add(category_goodChil);
        }
    }

    public async Task<bool> CheckExistInGoodsAsync(string code)
    {
        return await _context.Goods.AnyAsync(a => a.PriceList == code);
    }

    // IMPORT: giữ nguyên tên method & tham số
    public async Task ImportAsync(List<CategoryImport> categories)
    {
        foreach (var category in categories)
        {
            var categoryType = int.Parse(category.Type.Split("-")[0]);
            var categoryCheck = await _context.Categories.FirstOrDefaultAsync(x => x.Code == category.Code && x.Type == categoryType);
            categoryCheck ??= new Category();
            categoryCheck.Code = category.Code;
            categoryCheck.Type = categoryType;
            categoryCheck.Name = category.Name;

            // NEW: nếu CategoryImport có ProductCount, map vào; nếu không có thì giữ nguyên
            // (Không đổi tên thuộc tính/hàm Import)
            if (TryGetProductCount(category, out var pc))
            {
                categoryCheck.ProductCount = pc;
            }

            if (categoryCheck.Id > 0)
                _context.Categories.Update(categoryCheck);
            else
                _context.Categories.Add(categoryCheck);
        }
        await _context.SaveChangesAsync();
    }

    // Helper không đổi tên import, chỉ hỗ trợ đọc mềm dẻo (ProductCount hoặc productCount)
    private bool TryGetProductCount(CategoryImport src, out int? productCount)
    {
        productCount = null;
        try
        {
            // Ưu tiên property đúng tên nếu nó tồn tại trong DTO:
            // Giả định có thể đã thêm vào DTO CategoryImport: public int? ProductCount { get; set; }
            var prop = src.GetType().GetProperty("ProductCount");
            if (prop != null)
            {
                var val = prop.GetValue(src);
                if (val is int i) { productCount = i; return true; }
                if (val is int?) { productCount = (int?)val; return true; }
                if (val != null && int.TryParse(val.ToString(), out var parsed))
                {
                    productCount = parsed; return true;
                }
            }
            // fallback soft-name (nếu JSON có field productCount dạng dynamic)
            prop = src.GetType().GetProperty("productCount");
            if (prop != null)
            {
                var val = prop.GetValue(src);
                if (val is int i) { productCount = i; return true; }
                if (val is int?) { productCount = (int?)val; return true; }
                if (val != null && int.TryParse(val.ToString(), out var parsed))
                {
                    productCount = parsed; return true;
                }
            }
            return false;
        }
        catch { return false; }
    }

    public async Task<string> ExportAsync(int type)
    {
        string _fileMapServer = $"Category_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                _pathSave = Path.Combine(folder, _fileMapServer);
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\Category.xlsx");
        var datas = await _context.Categories.Where(x => type == 0 || x.Type == type).ToListAsync();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 7;
                int rowIdx = nRowBegin;
                if (datas.Count > 0)
                {
                    int i = 0;
                    foreach (var item in datas)
                    {
                        i++;
                        sheet.Cells[rowIdx, 1].Value = i.ToString();
                        sheet.Cells[rowIdx, 2].Value = item.Code;
                        sheet.Cells[rowIdx, 3].Value = item.Name;
                        sheet.Cells[rowIdx, 4].Value = item.Type + "-" + ((CategoryEnum)item.Type).GetDescription();
                        sheet.Cells[rowIdx, 5].Value = item.Note;

                        // Có thể ghi thêm cột 6 là ProductCount (tuỳ template):
                        // sheet.Cells[rowIdx, 6].Value = item.ProductCount ?? 0;

                        rowIdx++;
                    }
                    var loaiUse = sheet.Cells[nRowBegin, 4, rowIdx, 4].DataValidation.AddListDataValidation();
                    var categoryEnums = Enum.GetValues(typeof(CategoryEnum))
                            .Cast<CategoryEnum>()
                            .Select(v => new
                            {
                                code = v.ToString(),
                                name = v.GetDescription()
                            })
                            .ToList();
                    foreach (var categoryEnum in categoryEnums)
                    {
                        loaiUse.Formula.Values.Add(categoryEnum.code + "-" + categoryEnum.name);
                    }
                }

                rowIdx--;
                int nCol = 5;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }

            }
        }

        return _fileMapServer;
    }
}
