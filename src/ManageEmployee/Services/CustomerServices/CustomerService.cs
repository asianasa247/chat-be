using AutoMapper;
using Common.Constants;
using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services.CustomerServices;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;
    private readonly IMapper _mapper;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    public CustomerService(ApplicationDbContext context, IMapper mapper, IFileService fileService
        , IAccountBalanceSheetService accountBalanceSheetService,
        IChartOfAccountV2Service chartOfAccountV2Service)
    {
        _context = context;
        _fileService = fileService;
        _accountBalanceSheetService = accountBalanceSheetService;
        _mapper = mapper;
        _chartOfAccountV2Service = chartOfAccountV2Service;
    }

    public async Task<IEnumerable<CustomerModelView>> GetListCustomer(int type, List<long> customerId, string? searchText = null)
    {
        var customerQuery = from p in _context.Customers
                            join _cusClass in _context.CustomerClassifications on p.CustomerClassficationId equals _cusClass.Id into _custClassList
                            from listCustClassClist in _custClassList.DefaultIfEmpty()
                            join _pro in _context.Provinces on p.ProvinceId equals _pro.Id into _proList
                            from pros in _proList.DefaultIfEmpty()
                            join _dis in _context.Districts on p.ProvinceId equals _dis.Id into _disList
                            from districs in _disList.DefaultIfEmpty()
                            join _ward in _context.Wards on p.ProvinceId equals _ward.Id into _wardList
                            from wards in _wardList.DefaultIfEmpty()

                            join _cusTax in _context.CustomerTaxInformations on p.Id equals _cusTax.CustomerId into _cusTaxList
                            from cusTaxs in _cusTaxList.DefaultIfEmpty()
                            where p.Type == type
                            select new CustomerModelView
                            {
                                Id = p.Id,
                                Code = p.Code,
                                Name = p.Name,
                                Phone = cusTaxs.Phone,
                                IdentityCardNo = p.IdentityCardNo,
                                CustomerClassficationId = p.CustomerClassficationId,
                                CustomerClassficationName = listCustClassClist.Name,
                                DebitCode = p.DebitCode,
                                DebitDetailCodeFirst = p.DebitDetailCodeFirst,
                                DebitDetailCodeSecond = p.DebitDetailCodeSecond,
                                PriceList = p.PriceList,
                                Gender = p.Gender,
                                Address = string.IsNullOrEmpty(cusTaxs.Address) ? p.Address + ", " + wards.Name + ", " + districs.Name + ", " + pros.Name : cusTaxs.Address,
                                AccountNumber = cusTaxs.AccountNumber,
                                TaxCode = cusTaxs.TaxCode,
                                Bank = cusTaxs.Bank,
                                UserCreated = p.UserCreated
                            };
        var customers = await customerQuery.Where(p => string.IsNullOrEmpty(searchText) || p.Name.ToLower().Contains(searchText) || p.Code.ToLower().Contains(searchText)
                               || p.TaxCode != null && p.TaxCode.Contains(searchText)).Take(100).ToListAsync();

        var customerForIds = await customerQuery.Where(x => customerId.Contains(x.Id)).ToListAsync();
        return customers.Union(customerForIds);
    }

    public async Task<Customer> Create(Customer param)
    {
        var isValidate = await _context.Customers.AnyAsync(x => x.Phone == param.Phone);
        if (isValidate)
        {
            throw new ErrorException(ErrorMessages.CustomerAlreadyExist);
        }

        if (string.IsNullOrEmpty(param.Code))
        {
            param.Code = await GetCodeCustomer(param.Type);
        }
        await _context.Customers.AddAsync(param);
        try
        {
            param.Order = int.Parse(param.Code);
        }
        catch
        {
            param.Order = 0;
        }
        await _context.SaveChangesAsync();

        return param;
    }

    public async Task<string> Delete(int id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            // Xóa thông tin thuế
            List<CustomerTaxInformation> customerTax = await _context.CustomerTaxInformations.Where(o => o.CustomerId == id).ToListAsync();
            _context.RemoveRange(customerTax);

            // Xóa lịch sử liên hệ
            List<CustomerContactHistory> contactHistory = await _context.CustomerContactHistories.Where(o => o.CustomerId == id).ToListAsync();

            foreach (var item in contactHistory)
            {
                if (item.FileLink != "")
                {
                    string[] listFile = JsonConvert.DeserializeObject<string[]>(item.FileLink);

                    foreach (var file in listFile)
                    {
                        _fileService.DeleteFileUpload(file);
                    }
                }
            }

            _context.RemoveRange(contactHistory);

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
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

    public async Task<PagingResult<CustomerModelPaging>> GetPaging(CustomersSearchViewModel param, int userId, int year)
    {
        try
        {
            if (param.Page < 0)
                param.Page = 1;

            var user = await _context.Users.FindAsync(userId);
            string userRoleIds = "," + user?.UserRoleIds + ",";
            var roleCodes = await _context.UserRoles.Where(x => userRoleIds.Contains("," + x.Id.ToString() + ",")).Select(x => x.Code).ToListAsync();

            var customerIdFilters = new List<int>();
            if (param.JobId > 0 || param.StatusId > 0)
            {
                customerIdFilters = await _context.CustomerContactHistories.Where(X =>
                      (param.JobId == null || X.JobsId == param.JobId)
                      && (param.StatusId == null || X.StatusId == param.StatusId)
                      ).Select(x => x.CustomerId).Distinct().ToListAsync();
            }

            var listCustomers = from p in _context.Customers
                                where
                                                (!customerIdFilters.Any() || customerIdFilters.Contains(p.Id)) &&
                                                (param.Gender != null && param.Gender.Value != GenderEnum.All ? param.Gender.Value == p.Gender : true) &&
                                                (param.Birthday != null ? p.Birthday != null && param.Birthday.Value.Month == p.Birthday.Value.Month : true)
                                                     && (string.IsNullOrEmpty(param.SearchText) ||
                                                     p.Code.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                                     p.Phone.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                                     p.Name.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                                     p.IdentityCardNo.Trim().Contains(param.SearchText)
                                                     ) &&
                                                     (param.Gender == null || p.Gender == param.Gender)

                                                    && (param.FromDate == null || p.CreateAt >= param.FromDate)
                                                    && (param.ToDate == null || p.CreateAt <= param.ToDate)
                                                    && p.Type == param.Type
                                                    && (param.EmployeeId == null || param.EmployeeId == 0 || p.UserCreated == param.EmployeeId)
                                                    && (param.CustomerClassficationId == null || param.CustomerClassficationId == 0 || p.CustomerClassficationId == param.CustomerClassficationId)
                                                    && (param.ProvinceId == null || p.ProvinceId == param.ProvinceId)
                                orderby p.CreateAt descending
                                select new CustomerModelPaging
                                {
                                    Id = p.Id,
                                    Birthday = p.Birthday,
                                    Code = p.Code,
                                    Avatar = p.Avatar,
                                    Name = p.Name,
                                    Address = p.Address,
                                    Phone = p.Phone,
                                    DebitCode = p.DebitCode,
                                    DebitDetailCodeFirst = p.DebitDetailCodeFirst,
                                    DebitDetailCodeSecond = p.DebitDetailCodeSecond,
                                    UserCreated = p.UserCreated,
                                    TotalTask = 0,
                                    CreatedAt = p.CreateAt,
                                };

            if (param.EmployeeId == -1)
            {
                listCustomers = listCustomers.Where(x => x.UserCreated == null);
            }
            else
            {
                if (!roleCodes.Contains(UserRoleConst.SuperAdmin) && !roleCodes.Contains(UserRoleConst.AdminBranch) && !roleCodes.Contains(UserRoleConst.KeToanTruong))
                {
                    if (roleCodes.Contains(UserRoleConst.TruongPhong))
                    {
                        var userIds = await _context.Users.Where(x => x.DepartmentId == user.DepartmentId).Select(x => x.Id).ToListAsync();
                        listCustomers = listCustomers.Where(x => userIds.Contains(x.UserCreated ?? 0));
                    }
                    else
                    {
                        listCustomers = listCustomers.Where(x => x.UserCreated == userId);
                    }
                }
            }
            var customerCheckAccounts = await listCustomers.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();

            var customerDetails = GetCustomerDetails(param.JobId ?? 0)
                .ToDictionary(x => x.CustomerId);
            var customerIds = customerCheckAccounts.Select(x => x.Id);
            var listBill = _context.Bills.Where(x => customerIds.Contains(x.CustomerId) && x.CreatedDate.Year == DateTime.Now.Year);

            DateTime from = new(DateTime.Today.Year, 1, 1);
            DateTime to = DateTime.Today.AddDays(1);

            var balances = await _accountBalanceSheetService.GenerateReport(from, to, year,filterAccount:"131", isNoiBo: true);
            List<AccountBalanceItemViewModel> Items = balances
                .GroupBy(x => x.Hash)
                .Select(x => new AccountBalanceItemViewModel()
                {
                    AccountCode = x.First().AccountCode,
                    ArisingDebit = x.Sum(s => s.ArisingDebit),
                    ArisingCredit = x.Sum(s => s.ArisingCredit),
                    ParentRef = x.First().ParentRef,
                }).ToList();

            var listQuote = _context.CustomerQuote.Where(x => customerIds.Contains(x.CustomerId));
            var company = await _context.Companies.FirstOrDefaultAsync();

            for (int i = 0; i < customerCheckAccounts.Count; i++)
            {
                if (customerDetails.ContainsKey(customerCheckAccounts[i].Id))
                {
                    customerCheckAccounts[i].Details = customerDetails[customerCheckAccounts[i].Id];
                }
                customerCheckAccounts[i].TotalAmountPay = await listBill.Where(x => x.CustomerId == customerCheckAccounts[i].Id).SumAsync(x => x.TotalAmount);

                string accountCode = customerCheckAccounts[i].DebitCode;
                string accountParentCode = "";
                if (!string.IsNullOrEmpty(customerCheckAccounts[i].DebitDetailCodeSecond))
                {
                    accountCode = customerCheckAccounts[i].DebitDetailCodeSecond;
                    accountParentCode = $"{customerCheckAccounts[i].DebitCode}:{customerCheckAccounts[i].DebitDetailCodeFirst}";
                }
                else if (!string.IsNullOrEmpty(customerCheckAccounts[i].DebitDetailCodeFirst))
                {
                    accountParentCode = customerCheckAccounts[i].DebitCode;
                    accountCode = customerCheckAccounts[i].DebitDetailCodeFirst;
                }

                var accountItem = Items.Find(x => x.AccountCode == accountCode 
                            && (string.IsNullOrEmpty(accountParentCode) || x.ParentRef == accountParentCode));
                if (accountItem != null)
                    customerCheckAccounts[i].TotalAmountCN = accountItem.ArisingDebit - accountItem.ArisingCredit;

                customerCheckAccounts[i].CustomerQuoteCount = await listQuote.Where(x => x.CustomerId == customerCheckAccounts[i].Id).CountAsync();
                if (string.IsNullOrEmpty(customerCheckAccounts[i].Avatar))
                {
                    customerCheckAccounts[i].Avatar = company?.FileLogo;
                }
                customerCheckAccounts[i].TotalTask = await _context.UserTasks.CountAsync(x => x.CustomerId == customerCheckAccounts[i].Id);
            }

            return new PagingResult<CustomerModelPaging>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = listCustomers.Count(),
                Data = customerCheckAccounts
            };
        }
        catch
        {
            return new PagingResult<CustomerModelPaging>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<CustomerModelPaging>()
            };
        }
    }

    public async Task<List<TotalJobStatus>> GetTotalJobByUserId(int pageIndex, int pageSize, string keyword, int jobId, int statusId, int userId, int exportExcel = 0)
    {
        try
        {
            User _role = await _context.Users.Where(o => o.Id == userId).FirstOrDefaultAsync();

            List<TotalJobStatus> totalJobByUserId = await (from _job in _context.Jobs
                                                           join _history in _context.CustomerContactHistories on _job.Id equals _history.JobsId
                                                           join _cust in _context.Customers on _history.CustomerId equals _cust.Id
                                                           where (_role != null && _role.Timekeeper == 2 ? _cust.UserCreated > 0 : _cust.UserCreated == userId)
                                                               && (keyword != null && keyword.Length > 0 ?
                                                              _cust.Name.Trim().Contains(keyword) || _cust.Name.Trim().StartsWith(keyword) || _cust.Name.Trim().EndsWith(keyword) ||
                                                              _cust.Id.ToString().Contains(keyword) ||
                                                              _cust.Code.Trim().Contains(keyword) || _cust.Code.Trim().StartsWith(keyword) || _cust.Code.Trim().EndsWith(keyword) ||
                                                              _cust.Address.Trim().Contains(keyword) || _cust.Address.Trim().StartsWith(keyword) || _cust.Address.Trim().EndsWith(keyword) ||
                                                              _cust.Phone.Trim().Contains(keyword) || _cust.Phone.Trim().StartsWith(keyword) || _cust.Phone.Trim().EndsWith(keyword) ||
                                                              _cust.IdentityCardNo.Trim().Contains(keyword) || _cust.IdentityCardNo.Trim().StartsWith(keyword) || _cust.IdentityCardNo.Trim().EndsWith(keyword)
                                                               : _cust.Id != 0)
                                                           group _job by new { _job.Name, _job.Id, _job.Color } into _group
                                                           select new TotalJobStatus
                                                           {
                                                               Id = _group.Key.Id,
                                                               Name = _group.Key.Name,
                                                               Color = _group.Key.Color,
                                                               Amount = _group.Count()
                                                           }
                                ).ToListAsync();

            return totalJobByUserId;
        }
        catch
        {
            return new List<TotalJobStatus>();
        }
    }

    public async Task<List<TotalJobStatus>> GetTotalStatusByUserId(int pageIndex, int pageSize, string keyword, int jobId, int statusId, int userId, int exportExcel = 0)
    {
        try
        {
            User _role = await _context.Users.Where(o => o.Id == userId).FirstOrDefaultAsync();

            List<TotalJobStatus> totalJobByUserId = await (from _status in _context.Status
                                                           join _history in _context.CustomerContactHistories on _status.Id equals _history.StatusId
                                                           join _cust in _context.Customers on _history.CustomerId equals _cust.Id
                                                           where (_role != null && _role.Timekeeper == 2 ? _cust.UserCreated > 0 : _cust.UserCreated == userId)
                                                               && (keyword != null && keyword.Length > 0 ?
                                                              _cust.Name.Trim().Contains(keyword) || _cust.Name.Trim().StartsWith(keyword) || _cust.Name.Trim().EndsWith(keyword) ||
                                                              _cust.Id.ToString().Contains(keyword) ||
                                                              _cust.Code.Trim().Contains(keyword) || _cust.Code.Trim().StartsWith(keyword) || _cust.Code.Trim().EndsWith(keyword) ||
                                                              _cust.Address.Trim().Contains(keyword) || _cust.Address.Trim().StartsWith(keyword) || _cust.Address.Trim().EndsWith(keyword) ||
                                                              _cust.Phone.Trim().Contains(keyword) || _cust.Phone.Trim().StartsWith(keyword) || _cust.Phone.Trim().EndsWith(keyword) ||
                                                              _cust.IdentityCardNo.Trim().Contains(keyword) || _cust.IdentityCardNo.Trim().StartsWith(keyword) || _cust.IdentityCardNo.Trim().EndsWith(keyword)
                                                               : _cust.Id != 0)
                                                           group _status by new { _status.Name, _status.Id, _status.Color } into _group
                                                           select new TotalJobStatus
                                                           {
                                                               Id = _group.Key.Id,
                                                               Name = _group.Key.Name,
                                                               Color = _group.Key.Color,
                                                               Amount = _group.Count()
                                                           }
                                ).ToListAsync();

            return totalJobByUserId;
        }
        catch
        {
            return new List<TotalJobStatus>();
        }
    }

    public async Task<Customer> GetBySocialProviderId(string Provider, string ProviderId)
    {
        var result = await _context.Customers.FirstOrDefaultAsync(x => x.Provider == Provider && x.ProviderId == ProviderId);
      
        if (result == null)
        {
            return null;
        }
      
        return _mapper.Map<Customer>(result);
    }


    public async Task<CustomerGetterModel> GetById(int id, int year, bool isForBill = false)
    {
        var result = await _context.Customers.FindAsync(id);

        if (result == null)
        {
            return null;
        }

        var customer = _mapper.Map<CustomerGetterModel>(result);
        customer.Debit = await _chartOfAccountV2Service.FindAccount(result.DebitCode, string.Empty, year);
        customer.DebitDetailFirst = await _chartOfAccountV2Service.FindAccount(result.DebitDetailCodeFirst, result.DebitCode, year);
        customer.DebitDetailSecond = await _chartOfAccountV2Service.FindAccount(result.DebitDetailCodeSecond, result.DebitCode + ":" + result.DebitDetailCodeFirst, year);

        // taxCode
        var customerTax = await _context.CustomerTaxInformations.Where(x => x.CustomerId == id).FirstOrDefaultAsync();
        customer.TaxCode = customerTax?.TaxCode;
        if (isForBill)
        {
            var address = customerTax?.Address;
            if (string.IsNullOrEmpty(address))
            {
                var province = await _context.Provinces.FirstOrDefaultAsync(x => x.Id == customer.ProvinceId);
                var distric = await _context.Districts.FirstOrDefaultAsync(x => x.Id == customer.DistrictId);
                var ward = await _context.Wards.FirstOrDefaultAsync(x => x.Id == customer.WardId);
                address = $"{customer.Address}, {ward?.Name}, {distric?.Name}, {province?.Name}";
            }

            customer.Address = address;
        }
        return customer;
    }

    public async Task<Customer> Update(Customer model, string roles)
    {
        var isValidate = await _context.Customers.AnyAsync(x => x.Phone == model.Phone && x.Id != model.Id);
        if (isValidate)
        {
            throw new ErrorException(ErrorMessages.CustomerAlreadyExist);
        }

        var oldCustomers = await _context.Customers.FindAsync(model.Id);

        // check role
        //if ((oldCustomers?.UserCreated != model.UserUpdated) || ) && !roles.Contains(UserRoleConst.SuperAdmin) && !roles.Contains(UserRoleConst.AdminBranch))
        //{
        //    throw new ErrorException(ErrorMessage.CUSTOMER_UPDATE_PERMISSION);
        //}

        if ((oldCustomers?.UserCreated == model.UserUpdated)  || (roles.Contains(UserRoleConst.SuperAdmin)) || (roles.Contains(UserRoleConst.AdminBranch)))
        {
            try
            {
                oldCustomers.Order = int.Parse(oldCustomers.Code);
            }
            catch
            {
                oldCustomers.Order = 0;
            }
            oldCustomers.Address = model.Address;
            oldCustomers.Avatar = model.Avatar;
            oldCustomers.Birthday = model.Birthday;
            oldCustomers.DistrictId = model.DistrictId;
            oldCustomers.Email = model.Email;
            oldCustomers.Facebook = model.Facebook;
            oldCustomers.Gender = model.Gender;
            oldCustomers.IdentityCardAddressInCard = model.IdentityCardAddressInCard;
            oldCustomers.IdentityCardDistrictId = model.IdentityCardDistrictId;
            oldCustomers.IdentityCardIssueDate = model.IdentityCardIssueDate;
            oldCustomers.IdentityCardIssuePlace = model.IdentityCardIssuePlace;
            oldCustomers.IdentityCardNo = model.IdentityCardNo;
            oldCustomers.IdentityCardPlaceOfPermanent = model.IdentityCardPlaceOfPermanent;
            oldCustomers.IdentityCardProvinceId = model.IdentityCardProvinceId;
            oldCustomers.IdentityCardValidUntil = model.IdentityCardValidUntil;
            oldCustomers.IdentityCardWardId = model.IdentityCardWardId;
            oldCustomers.Code = model.Code;
            oldCustomers.Name = model.Name;
            oldCustomers.Phone = model.Phone;
            oldCustomers.ProvinceId = model.ProvinceId;
            oldCustomers.WardId = model.WardId;
            oldCustomers.UserUpdated = model.UserUpdated;

            oldCustomers.DebitCode = model.DebitCode;
            oldCustomers.DebitDetailCodeFirst = model.DebitDetailCodeFirst;
            oldCustomers.DebitDetailCodeSecond = model.DebitDetailCodeSecond;
            oldCustomers.CustomerClassficationId = model.CustomerClassficationId;
            oldCustomers.PriceList = model.PriceList;
            oldCustomers.Provider = model.Provider;
            oldCustomers.ProviderId = model.ProviderId;
            oldCustomers.EventCustomerId = model.EventCustomerId;

            _context.Customers.Update(oldCustomers);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ErrorException(ErrorMessage.CUSTOMER_UPDATE_PERMISSION);
        }    

        return model;
    }



    private List<CustomerWithDetail> GetCustomerDetails(int JobId)
    {
        var cusJobs = _context.Jobs.Join(_context.CustomerContactHistories, x => x.Id, y => y.JobsId, (x, y) => new
        {
            y.CustomerId,
            x.Name,
            x.Id,
            x.Color,
            y.StartTime,
            y.EndTime,
            y.StatusId
        })
          .Where(x => JobId > 0 ? x.Id == JobId : true)
        .ToList()
        .GroupBy(x => x.CustomerId)
        .Select(x => new
        {
            CustomerId = x.Key,
            LastJob = x.OrderByDescending(y => y.StartTime ?? DateTime.MinValue).First()
        })
        .ToDictionary(x => x.CustomerId);

        var customers = _context.Customers
            .Select(x => new
            {
                x.Id,
                x.UserCreated
            }).ToDictionary(x => x.Id);

        var users = _context.Users.Select(x => new
        {
            x.Id,
            Name = x.FullName
        }).ToDictionary(x => x.Id);

        var jobStatus = _context.Status.Select(x => new
        {
            x.Id,
            x.Name
        }).ToDictionary(x => x.Id);

        List<CustomerWithDetail> cusDetails =
            customers.Select(x => new CustomerWithDetail()
            {
                CustomerId = x.Key,
                CustomerOfUserId = x.Value.UserCreated ?? -1
            }).ToList();

        for (int i = 0; i < cusDetails.Count; i++)
        {
            if (users.ContainsKey(cusDetails[i].CustomerOfUserId))
            {
                cusDetails[i].CustomerOfUserFullName = users[cusDetails[i].CustomerOfUserId].Name;
            }

            if (cusJobs.ContainsKey(cusDetails[i].CustomerId))
            {
                cusDetails[i].LastJobId = cusJobs[cusDetails[i].CustomerId].LastJob.Id;
                cusDetails[i].LastJobName = cusJobs[cusDetails[i].CustomerId].LastJob.Name;
                cusDetails[i].LastJobStartTime = cusJobs[cusDetails[i].CustomerId].LastJob.StartTime ?? DateTime.MinValue;
                cusDetails[i].LastJobColor = cusJobs[cusDetails[i].CustomerId].LastJob.Color;
                cusDetails[i].LastJobStatusId = cusJobs[cusDetails[i].CustomerId].LastJob.StatusId.Value;

                if (jobStatus.ContainsKey(cusDetails[i].LastJobStatusId))
                {
                    cusDetails[i].LastJobStatusName = jobStatus[cusDetails[i].LastJobStatusId].Name;
                }
            }
        }

        return cusDetails;
    }


    public async Task<string> GetCodeCustomer(int Type)
    {
        var customer = await _context.Customers.Where(x => x.Code.Length == 8 && x.Type == Type).OrderByDescending(x => x.Code).FirstOrDefaultAsync();
        string code = customer?.Code;
        return GenaralCode(customer?.Code);
    }

    private string GenaralCode(string customerCode)
    {
        try
        {
            if (string.IsNullOrEmpty(customerCode))
                return "00000001";

            string userName = (int.Parse(customerCode) + 1).ToString();
            while (userName.Length < 8)
            {
                userName = "0" + userName;
            }
            return userName;
        }
        catch
        {
            return "00000001";
        }
    }
    public async Task<IEnumerable<CustomerWarning>> CustomerWarning()
    {
        var customers = await _context.Customers.OrderBy(x => x.UpdateHistoryContactAt).Select(x => _mapper.Map<CustomerWarning>(x)).Take(20).ToListAsync();
        foreach (var customer in customers)
        {
            customer.NumberPurchases = await _context.Bills.Where(x => x.CustomerId == customer.Id).CountAsync();
            customer.LastPurchasesAt = await _context.Bills.Where(x => x.CustomerId == customer.Id).OrderBy(x => x.CreatedDate).Select(x => x.CreatedDate).FirstOrDefaultAsync();
        }
        return customers;
    }

    public async Task<double> CustomerCnById(int id, int year)
    {
        var customer = await _context.Customers.FindAsync(id);
        var account = new ChartOfAccount();
        if (!string.IsNullOrEmpty(customer.DebitDetailCodeSecond))
            account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == customer.DebitDetailCodeSecond
            && x.ParentRef == customer.DebitCode + ":" + customer.DebitDetailCodeFirst);
        else if (!string.IsNullOrEmpty(customer.DebitDetailCodeFirst))
            account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == customer.DebitDetailCodeFirst
            && x.ParentRef == customer.DebitCode);
        else if (!string.IsNullOrEmpty(customer.DebitCode))
            account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == customer.DebitCode);
        if (account == null)
            return 0;
        return (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + ((account.ArisingDebit ?? 0) - (account.ArisingCredit ?? 0));
    }

    public async Task<IEnumerable<SelectListModel>> GetListCustomerWithCodeName()
    {
        List<SelectListModel> listCustomers = await (from p in _context.Customers
                                                     select new SelectListModel
                                                     {
                                                         Id = p.Id,
                                                         Code = p.Code,
                                                         Name = p.Code + " - " + p.Name,
                                                     }).ToListAsync();
        return listCustomers;
    }

    public async Task ValidateCustomer(Customer param)
    {
        var isExisted = await _context.Customers.AnyAsync(x => x.Type == param.Type && x.Id != param.Id && x.Code == param.Code);
        if (isExisted)
        {
            throw new ErrorException("Mã khách hàng đã tồn tại trong hệ thống");
        }
    }

    public async Task UpdateUserCreate(ChangeUserCreateRequest request)
    {
        if (request is null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
        }

        if (request.UserId == null)
        {
            throw new ErrorException(ErrorMessage.USERID_IS_EMPTY);
        }

        if (request.CustomerIds == null)
        {
            request.CustomerIds = new List<int>();
        }
        var customers = await _context.Customers.Where(x => request.CustomerIds.Contains(x.Id)).ToListAsync();
        customers = customers.ConvertAll(x => { x.UserCreated = request.UserId; return x; });
        _context.Customers.UpdateRange(customers);
        await _context.SaveChangesAsync();
    }

    public async Task SetAccountanForCustomer(int id, AccountanForCustomerModel form)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        customer.DebitCode = form.DebitCode;
        customer.DebitDetailCodeFirst = form.DebitDetailCodeFirst;
        customer.DebitDetailCodeSecond = form.DebitDetailCodeSecond;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task SyncAccountToCustomer(int type)
    {
        var parentRef = "131";
        if (type == 1)
        {
            parentRef = "331";
        }
        var accounts = await _context.ChartOfAccounts.Where(x => x.ParentRef.Contains(parentRef)
                                                && !x.HasDetails).ToListAsync();
        var customers = new List<Customer>();
        int i = 0;
        string customerCode = "";

        foreach (var account in accounts)
        {
            string accountCode2 = null;
            var accountCode1 = account.Code;
            var accountCode = account.ParentRef;
            if (account.Type == 6)
            {
                accountCode2 = account.Code;
                var code = account.ParentRef.Split(":");
                accountCode1 = code[1];
                accountCode = code[0];
            }
            var existCustomer = await _context.Customers.AnyAsync(x => x.DebitCode == accountCode && x.DebitDetailCodeFirst == accountCode1 &&
                                        (string.IsNullOrEmpty(accountCode2) || x.DebitDetailCodeSecond == accountCode2));
            if (existCustomer)
            {
                continue;
            }
            if (i  == 0)
            {
                customerCode = await GetCodeCustomer(0);
            }
            else
            {
                customerCode = GenaralCode(customerCode);
            }

            var customer = new Customer
            {
                DebitCode = accountCode,
                DebitDetailCodeFirst = accountCode1,
                DebitDetailCodeSecond = accountCode2,
                Code = customerCode,
                Name = account.Name,
                Type = type,
            };
            customers.Add(customer);
            i++;
        }
        await _context.AddRangeAsync(customers);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAcountFromCash(AccountanForCustomerModel form, int id, int year)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var accountParentCode = "";
        var accountFirstCode = "";
        var accountSecondCode = "";

        if (!string.IsNullOrEmpty(form.DebitCode))
        {
            accountParentCode = form.DebitCode;
            accountFirstCode = form.DebitDetailCodeFirst;
            accountSecondCode = form.DebitDetailCodeSecond;
        }
        else
        {
            accountParentCode = "131";

            if (customer.Type == 1)
            {
                accountParentCode = "331";
            }
            accountFirstCode = customer.Code;
        }
        var accountParent = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountParentCode);
        if (accountParent is null)
        {
            throw new ErrorException(string.Format(ErrorMessages.DataNotFoundWithData, $" tài khoản {accountParentCode}"));
        }
        var account = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.ParentRef == accountParentCode && x.Code == accountFirstCode);
        if(account != null && string.IsNullOrEmpty(form.DebitCode))
        {
            throw new ErrorException("Tài khoản thuộc mã khách hàng đã tồn tại ở kế toán vui lòng chọn tài khoản");

        }
        if (account is null)
        {
            account = new ChartOfAccount
            {
                Code = customer.Code,
                ParentRef = accountParentCode,
                Name = customer.Name,
                Duration = accountParent.Duration,
                Classification = accountParent.Classification,
                AccGroup = accountParent.AccGroup,
                Type = 5,
                DisplayInsert = true,
                DisplayDelete = true,
                Year = accountParent.Year,
                IsInternal = 1
            };
            await _context.ChartOfAccounts.AddAsync(account);

            accountParent.HasChild = true;
            accountParent.HasDetails = true;
            _context.ChartOfAccounts.Update(accountParent);
        }

        // update customer
        customer.DebitCode = accountParentCode;
        customer.DebitDetailCodeFirst = accountFirstCode;
        if (!string.IsNullOrEmpty(accountSecondCode))
        {
            customer.DebitDetailCodeSecond = accountSecondCode;
        }
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}