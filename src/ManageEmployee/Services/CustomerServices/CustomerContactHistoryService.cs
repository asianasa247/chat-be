using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using AutoMapper;

namespace ManageEmployee.Services.CustomerServices;

public class CustomerContactHistoryService : ICustomerContactHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CustomerContactHistoryService(ApplicationDbContext context, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerContactHistoryModel>> GetAll()
    {
        return await (from p in _context.CustomerContactHistories
                      join st in _context.Status on p.StatusId equals st.Id
                      join jb in _context.Jobs on p.JobsId equals jb.Id
                      where p.Id > 0
                      select new CustomerContactHistoryModel
                      {
                          Id = p.Id,
                          CustomerId = p.CustomerId,
                          Contact = p.Contact,
                          EndTime = p.EndTime,
                          JobsId = p.JobsId,
                          JobsName = jb.Name,
                          ExchangeContent = p.ExchangeContent,
                          FileLink = p.FileLink,
                          NextTime = p.NextTime,
                          Position = p.Position,
                          StartTime = p.StartTime,
                          StatusId = p.StatusId,
                          StatusName = st.Name,
                          StatusColor = st.Color,
                          JobColor = jb.Color
                      }).OrderByDescending(x => x.StartTime).ToListAsync();
    }

    public async Task<PagingResult<CustomerContactHistoryPagingModel>> GetByCustomerId(CustomersHistoryRequestModel param)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;

            var query = from p in _context.CustomerContactHistories
                        join cust in _context.Customers on p.CustomerId equals cust.Id
                        join st in _context.Status on p.StatusId equals st.Id
                        join jb in _context.Jobs on p.JobsId equals jb.Id
                        join _user in _context.Users on p.UserCreated equals _user.Id into users
                        from user in users.DefaultIfEmpty()
                        where p.CustomerId == (param.CustomerId != null ? param.CustomerId : 0)
                        && (param.FromDate != null ? ((DateTime)p.StartTime).Date >= ((DateTime)param.FromDate).Date : p.Id > 0)
                        && (param.ToDate != null ? ((DateTime)p.StartTime).Date <= ((DateTime)param.ToDate).Date : p.Id > 0)
                        && (param.JobId == null || p.JobsId == param.JobId)
                        && (param.Status == null || p.StatusId == param.Status)
                        && (string.IsNullOrEmpty(param.SearchText) || jb.Name.ToLower().Contains(param.SearchText.ToLower()))

                        select new CustomerContactHistoryPagingModel
                        {
                            Id = p.Id,
                            CustomerId = p.CustomerId,
                            CustomerName = cust.Name,
                            Contact = p.Contact,
                            EndTime = p.EndTime,
                            JobsId = p.JobsId,
                            JobsName = jb.Name,
                            ExchangeContent = p.ExchangeContent,
                            FileLink = p.FileLink,
                            NextTime = p.NextTime,
                            Position = p.Position,
                            StartTime = p.StartTime,
                            StatusId = p.StatusId,
                            StatusName = st.Name,
                            StatusColor = st.Color,
                            JobColor = jb.Color,
                            UserCreated = user.FullName,
                            UserCreatedImage = user.Avatar,
                        };

            var data = await query.OrderByDescending(x => x.Id).Skip(param.Page * param.PageSize).Take(param.PageSize).ToListAsync();
            foreach (var item in data)
            {
                if (item.FileLink != null && item.FileLink != "")
                {
                    item.FileLinkPaser = JsonSerializer.Deserialize<List<string>>(item.FileLink);
                }
            }

            return new PagingResult<CustomerContactHistoryPagingModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = await query.CountAsync(),
                Data = data
            };
        }
        catch
        {
            return new PagingResult<CustomerContactHistoryPagingModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<CustomerContactHistoryPagingModel>()
            };
        }
    }

    public async Task<PagingResult<CustomerContactHistoryModel>> GetAllPaging(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var query = from p in _context.CustomerContactHistories
                        join st in _context.Status on p.StatusId equals st.Id
                        join jb in _context.Jobs on p.JobsId equals jb.Id
                        where p.Id > 0
                           && (keyword != null && keyword.Length > 0 ?
                           p.Contact.Trim().Contains(keyword) || p.Position.Trim().StartsWith(keyword) ||
                           p.ExchangeContent.Trim().EndsWith(keyword)
                            : p.Id != 0)
                        select new CustomerContactHistoryModel
                        {
                            Id = p.Id,
                            CustomerId = p.CustomerId,
                            Contact = p.Contact,
                            EndTime = p.EndTime,
                            JobsId = p.JobsId,
                            JobsName = jb.Name,
                            ExchangeContent = p.ExchangeContent,
                            FileLink = p.FileLink,
                            NextTime = p.NextTime,
                            Position = p.Position,
                            StartTime = p.StartTime,
                            StatusId = p.StatusId,
                            StatusName = st.Name,
                            StatusColor = st.Color,
                            JobColor = jb.Color
                        };
            return new PagingResult<CustomerContactHistoryModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<CustomerContactHistoryModel>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<CustomerContactHistoryModel>()
            };
        }
    }

    public async Task<CustomerContactHistoryDetailModel> Create(CustomerContactHistoryDetailModel param, int userId)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var existingCustomers = await _context.Customers.SingleOrDefaultAsync(x => x.Id == param.CustomerId);
            if (existingCustomers == null)
            {
                throw new ErrorException(ErrorMessages.CustomerCodeNotExist);
            }
            existingCustomers.UpdateHistoryContactAt = DateTime.Now;
            _context.Customers.Update(existingCustomers);

            var existingJobs = await _context.Jobs.SingleOrDefaultAsync(x => x.Id == param.JobsId);
            if (existingJobs == null)
            {
                throw new ErrorException(ErrorMessages.JobCodeNotExist);
            }
            var item = _mapper.Map<CustomerContactHistory>(param);
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            item.UserCreated = userId;
            item.UserUpdated = userId;
            _context.CustomerContactHistories.Add(item);
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return param;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<CustomerContactHistoryDetailModel> Update(CustomerContactHistoryDetailModel param, int userId)
    {
        var data = await _context.CustomerContactHistories.SingleOrDefaultAsync(x => x.Id == param.Id);
        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        data.Contact = param.Contact;
        data.Position = param.Position;
        data.StartTime = param.StartTime;
        data.EndTime = param.EndTime;
        data.NextTime = param.NextTime;
        data.ExchangeContent = param.ExchangeContent;
        data.StatusId = param.StatusId;
        data.JobsId = param.JobsId;
        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = userId;

        if (param.FileLinkStr != null && param.FileLinkStr != "")
        {
            data.FileLink = param.FileLinkStr;
        }

        _context.CustomerContactHistories.Update(data);
        await _context.SaveChangesAsync();
        return param;
    }

    public async Task Delete(int id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            // Xóa lịch sử liên hệ
            CustomerContactHistory contactHistory = await _context.CustomerContactHistories.Where(o => o.Id == id).FirstOrDefaultAsync();

            if (contactHistory.FileLink != "")
            {
                List<string> listFile = JsonSerializer.Deserialize<List<string>>(contactHistory.FileLink);

                foreach (var file in listFile)
                {
                    _fileService.DeleteFileUpload(file);
                }
            }

            _context.Remove(contactHistory);

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    public async Task<List<SelectListContactForCustomer>> GetListContactForCustomer(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        var customerTaxIds = await _context.CustomerTaxInformations.Where(x => x.CustomerId == customerId).Select(x => x.Id).ToListAsync();
        var infos = await _context.CustomerTaxInformationAccountants.Where(x => customerTaxIds.Contains(x.CustomerTaxInformationId))
            .Select(x => new SelectListContactForCustomer
            {
                Name = x.Name,
                Contact = x.Phone,
                CustomerName = customer.Name,
                Position = x.Position
            })
            .ToListAsync();

        SelectListContactForCustomer customerContact = new()
        {
            Name = customer.Name,
            Contact = customer.Phone,
            CustomerName = customer.Name,
            Position = "Giám đóc"
        };
        infos.Add(customerContact);

        return infos;
    }
    public async Task AddContactForCustomer(SelectListContactForCustomer form, int customerId)
    {
        var customerTaxId = await _context.CustomerTaxInformations.Where(x => x.CustomerId == customerId).Select(x => x.Id).FirstOrDefaultAsync();

        var contact = new CustomerTaxInformationAccountant
        {
            Name = form.Name,
            Position = form.Position,
            CustomerTaxInformationId = customerTaxId,
            Phone = form.Contact
        };
        _context.CustomerTaxInformationAccountants.Add(contact);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountCustomerContact()
    {
        return await _context.CustomerContactHistories.Where(X => X.NextTime <= DateTime.Now).CountAsync();
    }
    public async Task<IEnumerable<CustomerContactHistoryNotificationModel>> GetAllCustomerContact()
    {
        return await _context.CustomerContactHistories
            .Join(_context.Customers,
            h => h.CustomerId,
            c => c.Id,
            (h, c) => new
            {
                contact = h,
                customerName = c.Name
            })
             .Join(_context.Jobs,
            h => h.contact.JobsId,
            c => c.Id,
            (h, c) => new
            {
                h.contact,
                h.customerName,
                jobName = c.Name
            })
             .Join(_context.Status,
            h => h.contact.StatusId,
            c => c.Id,
            (h, c) => new
            {
                h.contact,
                h.customerName,
                h.jobName,
                statusName = c.Name
            })
            .Where(X => X.contact.NextTime <= DateTime.Now).Select(x => new CustomerContactHistoryNotificationModel
            {
                Id = x.contact.Id,
                CustomerId = x.contact.CustomerId,
                CustomerName = x.customerName,
                Contact = x.contact.Contact,
                NextTime = x.contact.NextTime,
                ExchangeContent = x.contact.ExchangeContent,
                JobName = x.jobName,
                StatusName = x.statusName,
            }).ToListAsync();
    }

    public async Task<CustomerContactHistoryDetailModel> GetDetail(int id)
    {
        return _mapper.Map<CustomerContactHistoryDetailModel>(await _context.CustomerContactHistories.FindAsync(id));
    }
}