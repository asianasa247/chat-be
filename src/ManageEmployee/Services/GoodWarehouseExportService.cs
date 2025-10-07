using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class GoodWarehouseExportService : IGoodWarehouseExportService
{
    private readonly ApplicationDbContext _context;

    public GoodWarehouseExportService(ApplicationDbContext context)
    {
        _context = context;
    }
   
    public PagingResult<GoodWarehouseExportsViewModel> GetAll(GoodWarehouseExportRequestModel param)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;
            if (param.Fromdt == null)
                param.Fromdt = DateTime.Today;
            if (param.Todt == null)
                param.Todt = DateTime.Today;
            DateTime fromdt = new DateTime(param.Fromdt.Value.Year, param.Fromdt.Value.Month, param.Fromdt.Value.Day);
            DateTime todt = new DateTime(param.Todt.Value.Year, param.Todt.Value.Month, param.Todt.Value.Day).AddDays(1);
            var results = (from ex in _context.GoodWarehouseExport
                           join p in _context.GoodWarehouses on ex.GoodWarehouseId equals p.Id
                           where ex.CreatedAt > fromdt && ex.CreatedAt < todt
                           select new GoodWarehouseExportsViewModel()
                           {
                               Id = ex.Id,
                               Warehouse = p.Warehouse,
                               WarehouseName = p.WarehouseName,
                               Quantity = p.Quantity,
                               DateExpiration = p.DateExpiration,
                               Order = p.Order,
                               OrginalVoucherNumber = p.OrginalVoucherNumber,
                               QrCode = (!String.IsNullOrEmpty(p.Detail2) ? p.Detail2 : (p.Detail1 ?? p.Account)) + " " + p.Order + "-" + p.Id,
                               GoodCode = !String.IsNullOrEmpty(p.Detail2) ? p.Detail2 : (p.Detail1 ?? p.Account),
                               GoodName = !String.IsNullOrEmpty(p.DetailName2) ? p.DetailName2 : (p.DetailName1 ?? p.AccountName),
                           }).OrderByDescending(x => x.Id).ToList();
            if(param.Page == 0)
                return new PagingResult<GoodWarehouseExportsViewModel>()
                {
                    CurrentPage = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = results.Count,
                    Data = results
                };
            return new PagingResult<GoodWarehouseExportsViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = results.Count,
                Data = results.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToList()
            };
        }
        catch
        {
            return new PagingResult<GoodWarehouseExportsViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<GoodWarehouseExportsViewModel>()
            };
        }
    }
    public async Task<string> Delete(int BillId)
    {
        var goods = await _context.GoodWarehouseExport.Where(x => x.BillId == BillId).ToListAsync();
        if (goods.Count > 0)
        {
            _context.RemoveRange(goods);
            await _context.SaveChangesAsync();
        }
        return string.Empty;
    }
}