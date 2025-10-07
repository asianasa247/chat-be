using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.CustomerModels;

namespace ManageEmployee.Services.CustomerServices;
public class CustomerTaxInformationService : ICustomerTaxInformationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CustomerTaxInformationService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CustomerTaxInformation> Create(CustomerTaxInformationModel param)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var customerTaxInformation = _mapper.Map<CustomerTaxInformation>(param);
            // check exist
            var taxCheck = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == param.CustomerId);
            if (taxCheck is null)
            {
                _context.CustomerTaxInformations.Add(customerTaxInformation);
            }
            else
            {
                customerTaxInformation.Id = taxCheck.Id;
                _context.CustomerTaxInformations.Update(customerTaxInformation);
                var accountantDels = await _context.CustomerTaxInformationAccountants.Where(x => x.CustomerTaxInformationId == taxCheck.Id).ToListAsync();
                _context.CustomerTaxInformationAccountants.RemoveRange(accountantDels);
            }
            await _context.SaveChangesAsync();

            var accountants = param.Accountants.Select(x => new CustomerTaxInformationAccountant()
            {
                CustomerTaxInformationId = customerTaxInformation.Id,
                Phone = x.Phone,
                Position = x.Position,
                Name = x.Name
            }).ToList();

            await _context.CustomerTaxInformationAccountants.AddRangeAsync(accountants);
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();


            return customerTaxInformation;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<string> Delete(int id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var customer = await _context.CustomerTaxInformations.FindAsync(id);
            if (customer != null)
            {
                _context.CustomerTaxInformations.Remove(customer);
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

    public IEnumerable<CustomerTaxInformation> GetAll()
    {
        return _context.CustomerTaxInformations
                .OrderBy(x => x.Accountant);
    }

    public PagingResult<CustomerTaxInformation> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var listCustomers = from p in _context.CustomerTaxInformations
                                join cust in _context.Customers on p.CustomerId equals cust.Id
                                where p.Id > 0
                                && (keyword != null && keyword.Length > 0 ?
                                   p.CompanyName.Trim().Contains(keyword) || p.Address.Trim().StartsWith(keyword) ||
                                   p.TaxCode.Trim().EndsWith(keyword) || p.Bank.Trim().EndsWith(keyword) ||
                                   p.AccountNumber.Trim().EndsWith(keyword) || p.Position.Trim().EndsWith(keyword) ||
                                   p.PhoneOfAccountant.Trim().EndsWith(keyword)
                                    : p.Id != 0)
                                select new CustomerTaxInformation
                                {
                                    Id = p.Id,
                                    CustomerId = p.CustomerId,
                                    Accountant = p.Accountant,
                                    AccountNumber = p.AccountNumber,
                                    Address = p.Address,
                                    Bank = p.Bank,
                                    CompanyName = p.CompanyName,
                                    Phone = p.Phone,
                                    PhoneOfAccountant = p.PhoneOfAccountant,
                                    Position = p.Position,
                                    TaxCode = p.TaxCode
                                };
            return new PagingResult<CustomerTaxInformation>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = listCustomers.Count(),
                Data = listCustomers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
        }
        catch
        {
            return new PagingResult<CustomerTaxInformation>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<CustomerTaxInformation>()
            };

        }
    }

    public async Task<CustomerTaxInformationModel> GetById(int id)
    {
        var result = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == id);
        if (result is null)
            return new CustomerTaxInformationModel();

        CustomerTaxInformationModel itemOut = _mapper.Map<CustomerTaxInformationModel>(result);
        itemOut.Accountants = await _context.CustomerTaxInformationAccountants.Where(x => x.CustomerTaxInformationId == result.Id)
                            .Select(X => _mapper.Map<CustomerTaxInformationAccountantModel>(X)).ToListAsync();

        return itemOut;
    }
    public async Task<CustomerTaxInformationModel> GetByCustomerId(int customerId)
    {
        var result = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (result == null)
            return new CustomerTaxInformationModel();

        CustomerTaxInformationModel itemOut = _mapper.Map<CustomerTaxInformationModel>(result);
        itemOut.Accountants = await _context.CustomerTaxInformationAccountants.Where(x => x.CustomerTaxInformationId == result.Id)
                            .Select(X => _mapper.Map<CustomerTaxInformationAccountantModel>(X)).ToListAsync();

        return itemOut;
    }

    public async Task<CustomerTaxInformation> Update(CustomerTaxInformationModel model, int customerId)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            // Neu id = 0 or null thi them moi ban ghi
            var tax = await _context.CustomerTaxInformations.SingleOrDefaultAsync(x => x.CustomerId == customerId);
            if (tax == null)
            {
                tax = new CustomerTaxInformation();
            }

            tax.CustomerId = customerId;
            tax.Address = model.Address;
            tax.Phone = model.Phone;
            tax.PhoneOfAccountant = model.PhoneOfAccountant;
            tax.Position = model.Position;
            tax.TaxCode = model.TaxCode;
            tax.AccountNumber = model.AccountNumber;
            tax.Bank = model.Bank;
            tax.CompanyName = model.CompanyName;
            if (tax.Id > 0)
                _context.CustomerTaxInformations.Update(tax);
            else
            {
                _context.CustomerTaxInformations.Add(tax);
                await _context.SaveChangesAsync();
            }

            var accountantDels = await _context.CustomerTaxInformationAccountants.Where(x => x.CustomerTaxInformationId == tax.Id).ToListAsync();
            _context.CustomerTaxInformationAccountants.RemoveRange(accountantDels);

            var accountants = model.Accountants.Select(x => new CustomerTaxInformationAccountant()
            {
                CustomerTaxInformationId = tax.Id,
                Phone = x.Phone,
                Position = x.Position,
                Name = x.Name
            }).ToList();

            await _context.CustomerTaxInformationAccountants.AddRangeAsync(accountants);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return tax;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }
}
