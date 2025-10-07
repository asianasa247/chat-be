using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.P_ProcedureServices;

public class P_InventoryService : IP_InventoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public P_InventoryService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<P_InventoryViewModel>> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var result = new PagingResult<P_InventoryViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
            };

            var items = from i in _context.P_Inventories
                        join u in _context.Users on i.UserCreated equals u.Id into _u
                        from u in _u.DefaultIfEmpty()
                        where string.IsNullOrEmpty(keyword) ? true : i.Name.Contains(keyword)
                        orderby i.UpdatedAt descending
                        select new P_InventoryViewModel
                        {
                            Id = i.Id,
                            Name = i.Name,
                            CreateAt = i.CreatedAt,
                            ProcedureNumber = i.ProcedureNumber,
                            P_ProcedureStatusName = i.P_ProcedureStatusName,
                            UserCreatedName = u.FullName,
                            isFinish = i.isFinish
                        };

            result.TotalItems = items.Count();
            result.Data = await items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return result;
        }
        catch
        {
            return new PagingResult<P_InventoryViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<P_InventoryViewModel>()
            };
        }
    }

    public P_InventoryViewModel GetById(int id)
    {
        var Inventory = _mapper.Map<P_InventoryViewModel>(_context.P_Inventories.Find(id));
        Inventory.Items = _context.P_Inventory_Items.Where(x => x.P_InventoryId == id).Select(x => _mapper.Map<P_Inventory_Item_ViewModel>(x)).ToList();
        return Inventory;
    }

    public async Task<string> Create(P_InventoryViewModel param)
    {
        var inventory = _mapper.Map<P_Inventory>(param);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit("INVENTORY");
        inventory.P_ProcedureStatusId = status.Id;
        inventory.P_ProcedureStatusName = status.P_StatusName;
        _context.P_Inventories.Add(inventory);
        await _context.SaveChangesAsync();
        if (param.Items != null)
        {
            foreach (var inventoryItem in param.Items)
            {
                var item = _mapper.Map<P_Inventory_Item>(inventoryItem);
                item.P_InventoryId = inventory.Id;
                item.Id = 0;
                _context.P_Inventory_Items.Add(item);
            }
            await _context.SaveChangesAsync();
        }
        return "";
    }

    public async Task<string> Update(P_InventoryViewModel param)
    {
        var inventory = _mapper.Map<P_Inventory>(param);
        inventory.UpdatedAt = DateTime.Now;
        _context.P_Inventories.Update(inventory);
        var inventoryItemDel = await _context.P_Inventory_Items.Where(x => x.P_InventoryId == param.Id).ToListAsync();
        _context.P_Inventory_Items.RemoveRange(inventoryItemDel);
        if (param.Items != null)
        {
            foreach (var inventoryItem in param.Items)
            {
                var item = _mapper.Map<P_Inventory_Item>(inventoryItem);
                item.P_InventoryId = inventory.Id;
                item.Id = 0;
                _context.P_Inventory_Items.Add(item);
            }
        }
        await _context.SaveChangesAsync();
        return "";
    }

    public async Task<string> Delete(int id)
    {
        var inventory = await _context.P_Inventories.FindAsync(id);
        if (inventory != null)
        {
            _context.P_Inventories.Remove(inventory);
            var inventoryItemDel = await _context.P_Inventory_Items.Where(x => x.P_InventoryId == id).ToListAsync();
            _context.P_Inventory_Items.RemoveRange(inventoryItemDel);
            await _context.SaveChangesAsync();
        }
        return "";
    }

    public string GetProcedureNumber()
    {
        var item = _context.P_Inventories.OrderByDescending(x => x.ProcedureNumber).FirstOrDefault();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        if (item == null)
            return $"INVENTORY_{dt}_0000001";
        var procedureNumbers = item.ProcedureNumber.Split("_");
        try
        {
            var procedureNumber = procedureNumbers[2];
            while (true)
            {
                if (procedureNumber.Length > 7)
                {
                    break;
                }
                procedureNumber = "0" + procedureNumber;
            }
            return $"INVENTORY_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"INVENTORY_{dt}_0000001";
        }
    }

    public async Task<string> Accept(int id, int userId)
    {
        var inventory = _context.P_Inventories.Find(id);
        inventory.isFinish = true;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(inventory.P_ProcedureStatusId, userId);
        inventory.P_ProcedureStatusId = status.Id;
        inventory.P_ProcedureStatusName = status.P_StatusName;
        inventory.UpdatedAt = DateTime.Now;
        _context.P_Inventories.Update(inventory);
        var inventoryItems = _context.P_Inventory_Items.Where(x => x.P_InventoryId == id).ToList();
        foreach (var inventoryItem in inventoryItems)
        {
            int goodWareHouseId = int.Parse(inventoryItem.QrCode.Split("-")[1]);
            var goodWareHouse = _context.GoodWarehouses.FirstOrDefault(x => x.Id == goodWareHouseId && x.Quantity != inventoryItem.QuantityReal);
            if (goodWareHouse != null)
            {
                goodWareHouse.Quantity = inventoryItem.QuantityReal;
                _context.GoodWarehouses.Update(goodWareHouse);
            }
        }

        await _context.SaveChangesAsync();
        return "";
    }

    public IEnumerable<P_Inventory_Item_ViewModel> GetListGood(string wareHouse, string account, string detail1, string detail2)
    {
        var goods = _context.GoodWarehouses.Where(x => x.Quantity > 0
        && (string.IsNullOrEmpty(wareHouse) ? true : x.Warehouse == wareHouse)
        && (string.IsNullOrEmpty(account) ? true : x.Account == account)
        && (string.IsNullOrEmpty(detail1) ? true : x.Detail1 == detail1)
        && (string.IsNullOrEmpty(detail2) ? true : x.Detail2 == detail2)
        ).ToList();
        var goodIds = goods.Select(x => x.Id).ToList();
        var goodExports = _context.GoodWarehouseExport.Where(x => goodIds.Contains(x.GoodWarehouseId)).ToList();
        foreach (var good in goods)
        {
            P_Inventory_Item_ViewModel item = new P_Inventory_Item_ViewModel()
            {
                Id = 0,
                P_InventoryId = 0,
                OutputQuantity = goodExports.Where(x => x.GoodWarehouseId == good.Id).Sum(x => x.Quantity),
                Quantity = good.Quantity,
                QuantityReal = 0,
                QrCode = (!String.IsNullOrEmpty(good.Detail2) ? good.Detail2 : (good.Detail1 ?? good.Account)) + " " + good.Order + "-" + good.Id,
                GoodsCode = !String.IsNullOrEmpty(good.Detail2) ? good.Detail2 : (good.Detail1 ?? good.Account),
                GoodsName = !String.IsNullOrEmpty(good.Detail2) ? good.Detail2 : (good.Detail1 ?? good.Account),
            };
            yield return item;
        }
    }

    public string ExportInventoryById(int inventoryId)
    {
        try
        {
            string _fileMapServer = $"KiemKho_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                 folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                 _pathSave = Path.Combine(folder, _fileMapServer);

            P_InventoryViewModel inventory = GetById(inventoryId);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\InventoryExportId.xlsx");
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 7;
                    int rowIdx = nRowBegin;
                    sheet.Cells[3, 1].Value = "Ngày kiểm kê " + inventory.CreateAt.Value.ToString("dd/MM/yyyy");
                    sheet.Cells[4, 1].Value = "Người kiểm kê " + _context.Users.Find(inventory.UserCreated)?.FullName;

                    foreach (var item in inventory.Items)
                    {
                        sheet.Cells[rowIdx, 1].Value = rowIdx - 6;
                        sheet.Cells[rowIdx, 2].Value = item.QrCode;
                        sheet.Cells[rowIdx, 3].Value = item.GoodsCode;
                        sheet.Cells[rowIdx, 4].Value = item.GoodsName;
                        sheet.Cells[rowIdx, 5].Value = item.InputQuantity;
                        sheet.Cells[rowIdx, 6].Value = item.OutputQuantity;
                        sheet.Cells[rowIdx, 7].Value = item.Quantity;
                        sheet.Cells[rowIdx, 8].Value = item.QuantityReal;
                        sheet.Cells[rowIdx, 9].Value = item.Note;

                        rowIdx++;
                    }
                    rowIdx--;
                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 5, rowIdx, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
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
        catch
        {
            return string.Empty;
        }
    }
}