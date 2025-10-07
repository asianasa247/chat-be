using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.P_ProcedureServices;
public class P_KpiService : IP_KpiService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public P_KpiService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<P_KpiPagingViewModel>> GetPaging(P_KpiRequestModel param)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;


            var result = new PagingResult<P_KpiPagingViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
            };

            var items = from k in _context.P_Kpis
                        join _dep in _context.Departments on k.DepartmentId equals _dep.Id into _depList
                        from deps in _depList.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(param.SearchText) || k.Name.Contains(param.SearchText))
                        select new P_KpiPagingViewModel
                        {
                            Id = k.Id,
                            Name = k.Name,
                            CreateAt = k.CreatedAt,
                            ProcedureNumber = k.ProcedureNumber,
                            P_ProcedureStatusName = k.P_ProcedureStatusName,
                            Month = k.Month,
                            DepartmentName = deps.Name
                        };
           
            result.TotalItems = await items.CountAsync();
            result.Data = await items.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
            return result;
        }
        catch
        {
            return new PagingResult<P_KpiPagingViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<P_KpiPagingViewModel>()
            };
        }
    }
   
    public P_KpiViewModel GetById(int id)
    {
        var kpi = _mapper.Map<P_KpiViewModel>(_context.P_Kpis.Find(id));
        kpi.Items = _context.P_Kpi_Items.Where(x => x.P_KpiId == id).Select(x => _mapper.Map<P_Kpi_Item_ViewModel>(x)).ToList();
        var users = _context.Users.Where(x => x.DepartmentId == kpi.DepartmentId).ToList();
        foreach (var item in kpi.Items)
        {
            var user = users.FirstOrDefault(x => x.Id == item.UserId);
            if (user == null)
                continue;
            item.UserName = user.Username;
            item.FullName = user.FullName;
        }
        return kpi;
    }

    public async Task<string> Create(P_KpiViewModel param, int userId)
    {
        var kpi = _mapper.Map<P_Kpi>(param);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.KPI));
        kpi.P_ProcedureStatusId = status.Id;
        kpi.P_ProcedureStatusName = status.P_StatusName;
        kpi.UserUpdated = userId;
        kpi.UpdatedAt = DateTime.Now;
        kpi.UserCreated= userId;
        kpi.CreatedAt = DateTime.Now;

        _context.P_Kpis.Add(kpi);
        await _context.SaveChangesAsync();
        if(param.Items != null)
        {
            foreach (var kpiItem in param.Items)
            {
                var item = _mapper.Map<P_Kpi_Item>(kpiItem);
                item.P_KpiId = kpi.Id;
                item.Id = 0;
                _context.P_Kpi_Items.Add(item);
            }
            await _context.SaveChangesAsync();
        }
        return "";
    }

    public async Task<string> Update(P_KpiViewModel param, int userId)
    {
        var kpi = _mapper.Map<P_Kpi>(param);
        kpi.UserUpdated = userId;
        kpi.UpdatedAt = DateTime.Now;

        _context.P_Kpis.Update(kpi);
        var kpiItemDel = _context.P_Kpi_Items.Where(x => x.P_KpiId == param.Id).ToList();
        _context.P_Kpi_Items.RemoveRange(kpiItemDel);
        if (param.Items != null)
        {
            foreach (var kpiItem in param.Items)
            {
                var item = _mapper.Map<P_Kpi_Item>(kpiItem);
                item.P_KpiId = kpi.Id;
                item.Id = 0;
                _context.P_Kpi_Items.Add(item);
            }
        }
        await  _context.SaveChangesAsync();
        return "";
    }
    public async Task<string> Accept(P_KpiViewModel param, int userId)
    {
        var kpi = _mapper.Map<P_Kpi>(param);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(param.P_ProcedureStatusId, userId);
        kpi.P_ProcedureStatusId = status.Id;
        kpi.P_ProcedureStatusName = status.P_StatusName;
        if (!kpi.IsFinish)
            kpi.IsFinish = status.IsFinish;
        kpi.UserUpdated = userId;
        kpi.UpdatedAt = DateTime.Now;

        _context.P_Kpis.Update(kpi);
        var kpiItemDel = _context.P_Kpi_Items.Where(x => x.P_KpiId == param.Id).ToList();
        _context.P_Kpi_Items.RemoveRange(kpiItemDel);
        if (param.Items != null)
        {
            foreach (var kpiItem in param.Items)
            {
                var item = _mapper.Map<P_Kpi_Item>(kpiItem);
                item.P_KpiId = kpi.Id;
                item.Id = 0;
                _context.P_Kpi_Items.Add(item);
            }
        }
        await _context.SaveChangesAsync();
        return "";
    }
    public async Task<string> Delete(int id)
    {
        var kpi = _context.P_Kpis.Find(id);
        if (kpi != null)
        {
            _context.P_Kpis.Remove(kpi);
            var kpiItemDel = _context.P_Kpi_Items.Where(x => x.P_KpiId == id).ToList();
            _context.P_Kpi_Items.RemoveRange(kpiItemDel);
            await _context.SaveChangesAsync();
            await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.KPI));

        }
        return "";
    }

    public string GetProcedureNumber()
    {
        var item = _context.P_Kpis.OrderByDescending(x => x.ProcedureNumber).FirstOrDefault();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        if (item == null)
            return $"{nameof(ProcedureEnum.KPI)}_{dt}_0000001";
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
            return $"{nameof(ProcedureEnum.KPI)}_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"{nameof(ProcedureEnum.KPI)}_{dt}_0000001";
        }
    }

    public async Task<IEnumerable<ExportKPIUser>> ReportKPI(int? UserId, int? DepartmentId, int? BranchId, int Month)
    {
        var kpiIds = _context.P_Kpis.Where(x => x.Month == Month && x.IsFinish).Select(x => x.Id).ToList();
        var users = _context.Users.Where(x => !x.IsDelete && !x.Status
                    && (DepartmentId > 0 ? x.DepartmentId == DepartmentId : true)
                    && (BranchId > 0 ? x.BranchId == BranchId : true)).ToList();
        var userIds = users.Select(x => x.Id).ToList();
        var kpiItems = _context.P_Kpi_Items.Where(x => kpiIds.Contains(x.P_KpiId)).ToList();
        var menuKpis = _context.MenuKpis.ToList();

        // tinh tong thoi gian đi muộn về sớm
        var inoutData = await (from io in _context.InOutHistories
                               join sym in _context.Symbols on io.SymbolId equals sym.Id
                               join user in _context.Users on io.UserId equals user.Id
                               where io.TimeIn.Value.Month == Month && userIds.Contains(user.Id)
                               select new
                               {
                                   UserId = io.UserId,
                                   TimeIn = (io.TimeIn.Value - sym.TimeIn).Minutes,
                                   TimeOut = (sym.TimeOut - io.TimeOut.Value).Minutes,
                               }).ToListAsync();

        var inouts = inoutData
                 .GroupBy(x => x.UserId)
                 .Select(x => new
                 {
                     UserId = x.Key,
                     TotalTimeIn = x.Sum(t => t.TimeIn),
                     TotalTimeOut = x.Sum(t => t.TimeOut)
                 }) ;

        var tasks = _context.UserTasks.Where(x => x.CreatedDate.Value.Month == Month).ToList();

        var itemOut = new List<ExportKPIUser>();
        foreach (var user in users)
        {
            ExportKPIUser item = new ExportKPIUser();
            item.UserId = user.Id;
            item.UserName = user.Username;
            item.FullName = user.FullName;

            // điểm trong công việc
            var point = tasks.Where(x => x.UserCreated == user.Id).Sum(x => x.Point);

            var listkpiItem = kpiItems.Where(x => x.UserId == user.Id).ToList();
            if (listkpiItem.Any())
            {
                item.PointKpi = listkpiItem.Sum(x => x.Point);

                // tinh kpi dat duoc
                // diem doanh thu
                var bills = _context.Bills.Where(x => x.CreatedDate.Month == Month && x.UserCreated == user.Id);
                if (bills.Any())
                {
                    var total = bills.Sum(x => x.TotalAmount + (x.Surcharge ?? 0));
                    var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 1 && x.FromValue <= total && x.ToValue >= total );
                    point += menuKpi?.Point ?? 0;
                }
                // diem di muon ve som
                var inout = inouts.Where(x => x.UserId == item.UserId).FirstOrDefault();
                if (inout != null) 
                {
                    var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 0 && x.FromValue > inout.TotalTimeIn && x.ToValue > inout.TotalTimeOut);
                    point += menuKpi?.Point ?? 0;
                }
                
                item.Point = point;
                item.Percent = item?.PointKpi  >0 ?  Math.Round((item?.Point ?? 0) / (item?.PointKpi ?? 1) * 100, 2) : 0;
            }
            itemOut.Add(item);
        }
        return itemOut;
    }

    public IEnumerable<P_Kpi_Item_ViewModel> GetAllUserActive(int? DepartmentId)
    {

        var users = _context.Users.Where(x => !x.IsDelete
                    && (DepartmentId> 0 ? x.DepartmentId == DepartmentId : true)).Select(x => _mapper.Map<UserActiveModel>(x));
        foreach(var user in users)
        {
            P_Kpi_Item_ViewModel item = new();
            item.UserId = user.Id;
            item.FullName = user.FullName;
            item.UserName = user.Username;
            item.Point = 0;
            item.Percent = 0;
            yield return item;
        }
    }

    public async Task<string> ExportExcel_Report_Kpi(int? UserId, int? DepartmentId, int? BranchId, int Month)
    {
        try
        {
            string _fileMapServer = $"ReportKpi_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                 folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                 _pathSave = Path.Combine(folder, _fileMapServer);

            var datas = await ReportKPI(UserId, DepartmentId, BranchId, Month);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\P_ReportKpi.xlsx");
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 5;
                    int rowIdx = nRowBegin;
                    foreach (var item in datas)
                    {
                        sheet.Cells[rowIdx, 1].Value = rowIdx - 4;
                        sheet.Cells[rowIdx, 2].Value = item.UserName;
                        sheet.Cells[rowIdx, 3].Value = item.FullName;
                        sheet.Cells[rowIdx, 4].Value = item.DepartmentName;
                        sheet.Cells[rowIdx, 5].Value = item.PointKpi ?? 0;
                        sheet.Cells[rowIdx, 6].Value = item.Point ?? 0;
                        sheet.Cells[rowIdx, 7].Value = (item.Point ?? 0) / (item.PointKpi ?? 1);

                        rowIdx++;
                    }
                    rowIdx--;
                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 5, rowIdx, 7].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
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

    public async Task<ReportKpiAllItem> ReportKpiAll(int month)
    {
        ReportKpiAllItem item = new ReportKpiAllItem();
        var kpiIds = await _context.P_Kpis.Where(x => x.Month == month && x.IsFinish).Select(x => x.Id).ToListAsync();
        var kpiItems = await _context.P_Kpi_Items.Where(x => kpiIds.Contains(x.P_KpiId)).ToListAsync();
        item.Name = "Tổng công ty";
        item.Items = new List<ReportKpiAllItem>();
        var branchs = await _context.Branchs.Where(x => !x.IsDelete).ToListAsync();
        var menuKpis = await _context.MenuKpis.ToListAsync();

        // tinh tong thoi gian đi muộn về sớm
        var inoutData = await (from io in _context.InOutHistories
                            join sym in _context.Symbols on io.SymbolId equals sym.Id
                            join user in _context.Users on io.UserId equals user.Id
                            where io.TimeIn.Value.Month == month
                            select new
                            {
                                UserId = io.UserId,
                                TimeIn = (io.TimeIn.Value - sym.TimeIn).Minutes,
                                TimeOut = (sym.TimeOut - io.TimeOut.Value).Minutes,
                            }).ToListAsync()
                      ;
        var inouts = inoutData
                 .GroupBy(x => x.UserId)
                 .Select(x => new
                 {
                     UserId = x.Key,
                     TotalTimeIn = x.Sum(t => t.TimeIn),
                     TotalTimeOut = x.Sum(t => t.TimeOut)
                 });

        var tasks = await _context.UserTasks.Where(x => x.CreatedDate.Value.Month == month).ToListAsync();

        foreach (var branch in branchs)
        {
            ReportKpiAllItem itemBranch = new ReportKpiAllItem();
            itemBranch.Name = branch.Name;
            itemBranch.Items = new List<ReportKpiAllItem>();
            var departments = await _context.Departments.Where(x => !x.isDelete && x.BranchId == branch.Id).ToListAsync();
            foreach(var department in departments)
            {
                ReportKpiAllItem itemDepartment = new ReportKpiAllItem();
                itemDepartment.Name = department.Name;
                itemDepartment.Items = new List<ReportKpiAllItem>();
                var users = await _context.Users.Where(x => x.DepartmentId == department.Id && !x.IsDelete).ToListAsync();
                foreach(var user in users)
                {
                    var kpi = kpiItems.FirstOrDefault(x => x.UserId == user.Id);
                    ReportKpiAllItem itemUser = new()
                    {
                        Name = user.FullName,
                        PointKpi = kpi?.Point ?? 0,
                    };

                    var point = tasks.Where(x => x.UserCreated == user.Id).Sum(x => x.Point);

                    // diem doanh thu
                    var bills = _context.Bills.Where(x => x.CreatedDate.Month == month && x.UserCreated == user.Id);
                    if (bills.Any())
                    {
                        var total = bills.Sum(x => x.TotalAmount + (x.Surcharge ?? 0));
                        var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 1 && x.FromValue <= total && x.ToValue >= total);
                        point += menuKpi?.Point ?? 0;
                    }
                    // diem di muon ve som
                    var inout = inouts.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (inout != null)
                    {
                        var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 0 && x.FromValue > inout.TotalTimeIn && x.ToValue > inout.TotalTimeOut);
                        point += menuKpi?.Point ?? 0;
                    }
                    itemUser.Point = point;

                    itemUser.Percent = itemUser.PointKpi > 0 ?  Math.Round((itemUser?.Point ?? 0) / (itemUser?.PointKpi ?? 1) * 100, 2) : 0;

                    itemDepartment.Items.Add(itemUser);
                }
                itemDepartment.PointKpi = itemDepartment.Items.Sum(x => x.PointKpi ?? 0);
                itemDepartment.Point = itemDepartment.Items.Sum(x => x.Point ?? 0);
                if (itemDepartment.PointKpi > 0)
                    itemDepartment.Percent = Math.Round((itemDepartment.Point ?? 0) / (itemDepartment.PointKpi ?? 1) * 100, 2);
                itemBranch.Items.Add(itemDepartment);
            }
            itemBranch.PointKpi = itemBranch.Items.Sum(x => x.PointKpi ?? 0);
            itemBranch.Point = itemBranch.Items.Sum(x => x.Point ?? 0);
            if (itemBranch.PointKpi > 0)
                itemBranch.Percent = Math.Round((itemBranch.Point ?? 0) / (itemBranch.PointKpi ?? 1) * 100, 2);

            item.Items.Add(itemBranch);
        }
        item.PointKpi = item.Items.Sum(x => x.PointKpi ?? 0);
        item.Point = item.Items.Sum(x => x.Point ?? 0);
        if (item.PointKpi > 0)
            item.Percent = Math.Round((item.Point ?? 0) / (item.PointKpi ?? 1) * 100, 2);

        return item;
    }

    public async Task<double> GetPointForUser(int userId, int month)
    {
        var menuKpis = await _context.MenuKpis.ToListAsync();
        var point = await _context.UserTasks.Where(x => x.CreatedDate.Value.Month == month && x.UserCreated == userId).SumAsync(x => x.Point);
        var inoutData = await (from io in _context.InOutHistories
                               join sym in _context.Symbols on io.SymbolId equals sym.Id
                               join user in _context.Users on io.UserId equals user.Id
                               where io.TimeIn.Value.Month == month && io.UserId == userId
                               select new
                               {
                                   UserId = io.UserId,
                                   TimeIn = (io.TimeIn.Value - sym.TimeIn).Minutes,
                                   TimeOut = (sym.TimeOut - io.TimeOut.Value).Minutes,
                               }).ToListAsync()
                     ;
        var inout = inoutData
                 .GroupBy(x => x.UserId)
                 .Select(x => new
                 {
                     UserId = x.Key,
                     TotalTimeIn = x.Sum(t => t.TimeIn),
                     TotalTimeOut = x.Sum(t => t.TimeOut)
                 }).FirstOrDefault();

        // diem doanh thu
        var billAmounts = await _context.Bills.Where(x => x.CreatedDate.Month == month && x.UserCreated == userId).SumAsync(x => x.TotalAmount + (x.Surcharge ?? 0));
        if (billAmounts > 0)
        {
            var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 1 && x.FromValue <= billAmounts && x.ToValue >= billAmounts);
            point += menuKpi?.Point ?? 0;
        }
        // diem di muon ve som
        if (inout != null)
        {
            var menuKpi = menuKpis.FirstOrDefault(x => x.Type == 0 && x.FromValue > inout.TotalTimeIn && x.ToValue > inout.TotalTimeOut);
            point += menuKpi?.Point ?? 0;
        }
        // luong khach hang
        var customerCount = await _context.Customers.Where(x => x.UserCreated == userId && x.CreateAt.Month == month).CountAsync();
        point += customerCount;

        return point; 
    }
    
}