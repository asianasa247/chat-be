using AutoMapper;
using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.P_ProcedureServices;
public class P_SalaryAdvanceService : IP_SalaryAdvanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ILedgerService _ledgerServices;
    public P_SalaryAdvanceService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService, ILedgerService ledgerServices)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _ledgerServices = ledgerServices;
    }
    public async Task<PagingResult<P_SalaryAdvancePagingViewModel>> GetAll(P_SalaryAdvanceRequestModel param, List<string> RoleName, int UserId)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;


            var result = new PagingResult<P_SalaryAdvancePagingViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
            };

            var items = _context.P_SalaryAdvance.Where(x => x.Id != 0)
                            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText))
                            .Where(x => (param.DepartmentId > 0) ? x.DepartmentId == param.DepartmentId : true)
                            .Where(x => (param.BranchId > 0) ? x.BranchId == param.BranchId : true)
                            .Where(x => (param.Month > 0) ? x.Date.Value.Month == param.Month : true)
                            .Select(x => new P_SalaryAdvancePagingViewModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                CreateAt = x.CreatedAt,
                                BranchId = x.BranchId,
                                DepartmentId = x.DepartmentId,
                                Date = x.Date,
                                ProcedureNumber = x.ProcedureNumber,
                                P_ProcedureStatusName = x.ProcedureStatusName,
                                IsFinish = x.IsFinished
                            });
            if (RoleName.Contains(UserRoleConst.SuperAdmin))
            {
            }
            else if (RoleName.Contains(UserRoleConst.AdminBranch))
            {
                int branchId = _context.Users.Find(UserId)?.BranchId ?? 0;
                items = items.Where(x => x.BranchId == branchId);
            }
            else if (RoleName.Contains(UserRoleConst.TruongPhong))
            {
                int departmentId = _context.Users.Find(UserId)?.DepartmentId ?? 0;

                items = items.Where(x => x.DepartmentId == departmentId);
            }
            else if (RoleName.Contains(UserRoleConst.NhanVien))
            {
            }
            result.TotalItems = await items.CountAsync();
            result.Data = await items.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
            return result;
        }
        catch
        {
            return new PagingResult<P_SalaryAdvancePagingViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<P_SalaryAdvancePagingViewModel>()
            };
        }
    }

    public async Task<string> Create(P_SalaryAdvanceViewModel request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var tamUng = _mapper.Map<P_SalaryAdvance>(request);
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.SALARY_ADVANCE));
            tamUng.ProcedureStatusId = status.Id;
            tamUng.ProcedureStatusName = status.P_StatusName;
            _context.P_SalaryAdvance.Add(tamUng);
            _context.SaveChanges();
            if (request.Items != null)
            {
                var listItem = request.Items.Select(x => { x.P_SalaryAdvanceId = tamUng.Id; return _mapper.Map<P_SalaryAdvance_Item>(x); }).ToList();
                _context.P_SalaryAdvance_Item.AddRange(listItem);
            }
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public P_SalaryAdvanceViewModel GetById(int id)
    {
        try
        {
            var item = _context.P_SalaryAdvance.Find(id);
            if (item != null)
            {
                var itemOut = _mapper.Map<P_SalaryAdvanceViewModel>(item);
                itemOut.Items = _context.P_SalaryAdvance_Item.Where(x => x.P_SalaryAdvanceId == id).Select(x => _mapper.Map<P_SalaryAdvance_ItemViewModel>(x)).ToList();
                return itemOut;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Update(P_SalaryAdvanceViewModel request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var tamUng = _context.P_SalaryAdvance.Find(request.Id);
            tamUng.Name = request.Name;
            tamUng.Date = request.Date;
            tamUng.BranchId = request.BranchId;
            tamUng.DepartmentId = request.DepartmentId;
            _context.P_SalaryAdvance.Update(tamUng);
            var listItemDel = _context.P_SalaryAdvance_Item.Where(x => x.P_SalaryAdvanceId == request.Id).ToList();
            _context.P_SalaryAdvance_Item.RemoveRange(listItemDel);
            if (request.Items != null)
            {
                var listItem = request.Items.Select(x => { x.P_SalaryAdvanceId = tamUng.Id; return _mapper.Map<P_SalaryAdvance_Item>(x); }).ToList();
                _context.P_SalaryAdvance_Item.AddRange(listItem);
            }
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }
    public async Task<string> Accept(P_SalaryAdvanceViewModel request, int userId)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var tamUng = await _context.P_SalaryAdvance.FindAsync(request.Id);
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(request.P_ProcedureStatusId, userId);
            tamUng.ProcedureStatusId = status.Id;
            tamUng.ProcedureStatusName = status.P_StatusName;

            //if (!tamUng.IsFinish)
            //    tamUng.IsFinish = status.IsFinish;
            tamUng.IsFinished = true;

            tamUng.Name = request.Name;
            tamUng.Date = request.Date;
            tamUng.BranchId = request.BranchId;
            tamUng.DepartmentId = request.DepartmentId;
            _context.P_SalaryAdvance.Update(tamUng);
            var listItemDel = await _context.P_SalaryAdvance_Item.Where(x => x.P_SalaryAdvanceId == request.Id).ToListAsync();
            _context.P_SalaryAdvance_Item.RemoveRange(listItemDel);
            if (request.Items != null)
            {
                var listItem = request.Items.Select(x => { x.P_SalaryAdvanceId = tamUng.Id; return _mapper.Map<P_SalaryAdvance_Item>(x); }).ToList();
                await _context.P_SalaryAdvance_Item.AddRangeAsync(listItem);
            }
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<string> Delete(int id)
    {
        var tamUng = await _context.P_SalaryAdvance.FindAsync(id);
        _context.P_SalaryAdvance.Remove(tamUng);
        var listItem = await _context.P_SalaryAdvance_Item.Where(x => x.P_SalaryAdvanceId == id).ToListAsync();
        _context.P_SalaryAdvance_Item.RemoveRange(listItem);
        await _context.SaveChangesAsync();
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.SALARY_ADVANCE));

        return string.Empty;
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.P_SalaryAdvance.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        if (item == null)
            return $"{nameof(ProcedureEnum.SALARY_ADVANCE)}_{dt}_0000001";
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
            return $"{nameof(ProcedureEnum.SALARY_ADVANCE)}_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"{nameof(ProcedureEnum.SALARY_ADVANCE)}_{dt}_0000001";
        }
    }

    public async Task AddLedger(int month, int year, int isInternal)
    {
        var salaryAdvanceIds = await _context.P_SalaryAdvance.Where(x => x.Date.Value.Month == month && x.IsFinished).Select(x => x.Id).ToListAsync();
        var salaryUsers = await _context.P_SalaryAdvance_Item.Where(x => salaryAdvanceIds.Contains(x.P_SalaryAdvanceId))
            .Join(_context.Users,
                    s => s.UserId,
                    u => u.Id,
                    (s, u) => new
                    {
                        salary = s,
                        user = u
                    })
            .GroupJoin(_context.PositionDetails,
                    s => s.user.PositionDetailId,
                    p => p.Id,
                    (s, p) => new
                    {
                        salary = s.salary,
                        user = s.user,
                        position = p
                    })
            .SelectMany(x => x.position.DefaultIfEmpty(),
                    (s, p) => new
                    {
                        isManager = p.IsManager,
                        salary = s.salary.Value,
                        userId = s.user.Id,
                    })
            .ToListAsync();

        var listSalarySocial = await _context.SalarySocials.Where(x => x.Code == "LUONGQUANLY" || x.Code == "LUONGNHANVIEN").ToListAsync();

        int maxOriginalVoucher = 0;
        
            var ledgerExist = await _context.GetLedger(year, isInternal).AsNoTracking().Where(x => x.IsDelete != true && x.Type == "KC"
                                                                && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();

            if (ledgerExist != null && ledgerExist.Count > 0)
            {
                maxOriginalVoucher = ledgerExist.Max(x => Int32.Parse(x.OrginalVoucherNumber.Split('-').Last()));
            }
        maxOriginalVoucher++;

        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);

        string orginalVoucherNumber = $"{"KC"}{(month < 10 ? "0" + month : month.ToString())}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";

        foreach (var salarySocial in listSalarySocial)
        {
            Ledger ledger = new()
            {
                Month = month,
                BookDate = DateTime.Today,
                Type = "KC"
            };
            ledger.VoucherNumber = (month < 10 ? "0" + month : month.ToString()) + "/" + ledger.Type;
            ledger.IsVoucher = false;

            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.OrginalBookDate = DateTime.Today;
            ledger.ReferenceBookDate = DateTime.Today;
            ledger.InvoiceDate = DateTime.Today;

            ledger.DebitCode = salarySocial.AccountDebit;
            ledger.DebitDetailCodeFirst = salarySocial.DetailDebit1;
            ledger.DebitDetailCodeSecond = salarySocial.DetailDebit2;
            ledger.CreditCode = salarySocial.AccountCredit;
            ledger.CreditDetailCodeFirst = salarySocial.DetailCredit1;
            ledger.CreditDetailCodeSecond = salarySocial.DetailCredit2;

            ledger.Quantity = 0;
            ledger.UnitPrice = 0;
            if (salarySocial.Code == "LUONGQUANLY")
                ledger.Amount = salaryUsers.Where(x => x.isManager).Sum(x => x.salary);
            else
                ledger.Amount = salaryUsers.Where(x => !x.isManager).Sum(x => x.salary);

            ledger.CreateAt = DateTime.Now;
            ledger.IsInternal = isInternal;
            ledger.OrginalCode = "";
            ledger.OrginalFullName = "";

            ledger.ReferenceVoucherNumber = "";
            ledger.ReferenceFullName = "";
            ledger.ReferenceAddress = "";
            ledger.InvoiceProductItem = "";
            ledger.ProjectCode = "";

           await  _ledgerServices.Create(ledger, year);
        }
    }

    public async Task<string> CreateForUser(P_SalaryAdvance_ItemForUser request, int userId)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var user = await _context.Users.FindAsync(userId);

            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.SALARY_ADVANCE));
            var procedure = await GetProcedureNumber();
            var tamUng = new P_SalaryAdvance()
            {
                ProcedureStatusId = status.Id,
                ProcedureStatusName = status.P_StatusName,
                Name = string.Format("Tạm ứng lương tháng {0}/{1}", DateTime.Now.Month, DateTime.Now.Year),
                ProcedureNumber = procedure,
                BranchId = user?.BranchId,
                DepartmentId = user?.DepartmentId,
                Date = DateTime.UtcNow,
                IsForUser = true,
                UserCreated = userId,
                UserUpdated = userId,
            };

            _context.P_SalaryAdvance.Add(tamUng);
            await _context.SaveChangesAsync();

            var tamUngItem = new P_SalaryAdvance_Item
            {
                P_SalaryAdvanceId = tamUng.Id,
                UserId = userId,
                BranchId = user?.BranchId ?? 0,
                Value = request.Value
            };
            _context.P_SalaryAdvance_Item.Add(tamUngItem);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }
    public async Task<PagingResult<P_SalaryAdvancePagingViewModelForUser>> GetAllForUser(PagingRequestModel param, int userId)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;


            var result = new PagingResult<P_SalaryAdvancePagingViewModelForUser>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
            };

            var items = _context.P_SalaryAdvance.Where(x => x.IsForUser && x.UserCreated == userId)
                            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText))
                            .Select(x => new P_SalaryAdvancePagingViewModelForUser
                            {
                                Id = x.Id,
                                Name = x.Name,
                                BranchId = x.BranchId,
                                DepartmentId = x.DepartmentId,
                                Date = x.Date,
                                ProcedureNumber = x.ProcedureNumber,
                                P_ProcedureStatusName = x.ProcedureStatusName
                            })
                            .OrderByDescending(x => x.Id);

            result.TotalItems = await items.CountAsync();
            var datas = await items.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
            var branchs = await _context.Branchs.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            foreach (var data in datas)
            {
                data.BranchName = branchs.Find(x => x.Id == data.BranchId)?.Name;
                data.DepartmentName = departments.Find(x => x.Id == data.DepartmentId)?.Name;
                data.Value = await _context.P_SalaryAdvance_Item.Where(X => X.P_SalaryAdvanceId == data.Id).Select(x => x.Value).FirstOrDefaultAsync();
            }
            result.Data = datas;
            return result;
        }
        catch
        {
            return new PagingResult<P_SalaryAdvancePagingViewModelForUser>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<P_SalaryAdvancePagingViewModelForUser>()
            };
        }
    }
    public async Task<P_SalaryAdvance_ItemForUser> GetByIdForUser(int id)
    {
        var salary = await _context.P_SalaryAdvance.FindAsync(id);
        var salaryItem = await _context.P_SalaryAdvance_Item.FirstOrDefaultAsync(x => x.P_SalaryAdvanceId == id);
        if (salary != null)
        {
            var itemOut = new P_SalaryAdvance_ItemForUser()
            {
                Id = id,
                Value = salaryItem.Value,
                UserId = salaryItem.UserId,
            };
            return itemOut;
        }
        return null;
    }

    public async Task<string> UpdateForUser(P_SalaryAdvance_ItemForUser request)
    {
        var item = await _context.P_SalaryAdvance_Item.FirstOrDefaultAsync(x => x.P_SalaryAdvanceId == request.Id);
        item.Value = request.Value;
        _context.P_SalaryAdvance_Item.Update(item);

        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task NotAccept(int id, int userId)
    {

        var procedure = await _context.P_SalaryAdvance.FindAsync(id);
        if (procedure.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        procedure.NoteNotAccept = procedure.ProcedureStatusName + "; " + procedure.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.SALARY_ADVANCE));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.UpdatedAt = DateTime.Now;
        _context.P_SalaryAdvance.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.SALARY_ADVANCE), status.Id, userId, id, procedure.ProcedureNumber, true);
    }

}
