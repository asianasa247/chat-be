using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Services.Interfaces.Orders;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class PayerServices : IPayerServices
{
    private readonly ApplicationDbContext _context;

    public PayerServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payer>> GetAll()
    {
        return await _context.Payers.ToListAsync();
    }

    public async Task<IEnumerable<PayerModelView>> GetAll(string searchText = "")
    {
        List<PayerModelView> listCustomers = await (from p in _context.Customers
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
                                                       where (string.IsNullOrEmpty(searchText) || p.Name.ToLower().Contains(searchText))
                                                       select new PayerModelView
                                                       {
                                                           Id = p.Id,
                                                           Code = p.Code,
                                                           Name = p.Name,
                                                           Phone = cusTaxs.Phone,
                                                           Address = String.IsNullOrEmpty(cusTaxs.Address) ? (p.Address + ", " + wards.Name + ", " + districs.Name + ", " + pros.Name) : cusTaxs.Address,
                                                           IdentityCardNo = p.IdentityCardNo,
                                                           TaxCode = cusTaxs.TaxCode,
                                                       }).ToListAsync();
        var payers = await _context.Payers.Where(x => x.PayerType == 1 
             && (string.IsNullOrEmpty(searchText) || x.Name.ToLower().Contains(searchText)))
            .Select(p => new PayerModelView
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Phone = p.Phone,
                Address = p.Address,
                IdentityCardNo = p.IdentityNumber,
                TaxCode = p.TaxCode,
            }).ToListAsync();
        return payers.Concat(listCustomers).Skip(0).Take(100);
    }

    public async Task<PagingResult<Payer>> GetListPayerWithCustomerTax(PayerPagingationRequestModel param)
    {
        var customerTaxs = await _context.CustomerTaxInformations.Where(x => !string.IsNullOrEmpty(x.TaxCode)
                            && (string.IsNullOrEmpty(param.SearchText) || x.TaxCode.Contains(param.SearchText) 
                                || x.CompanyName.ToUpper().Contains(param.SearchText.ToUpper())
                                ))
            .Select(x => new Payer()
            {
                TaxCode = x.TaxCode,
                Address = x.Address,
                Name = x.CompanyName
            }).ToListAsync();

        var payers = await _context.Payers.Where(x => string.IsNullOrEmpty(param.SearchText) || x.TaxCode.Contains(param.SearchText)
                                                    || x.Name.ToUpper().Contains(param.SearchText.ToUpper()))
            .Where(p => p.PayerType == param.PayerType)
            .Select(x => new Payer()
            {
                TaxCode = x.TaxCode,
                Address = x.Address,
                Name = x.Name
            })
            .ToListAsync();

        payers.AddRange(customerTaxs);
        payers = payers.DistinctBy(x => x.TaxCode).ToList();

        PagingResult<Payer> response = new PagingResult<Payer>()
        {
            Data = payers.Skip((param.Page - 1) * param.PageSize)
               .Take(param.PageSize).ToList(),
            TotalItems = payers.Count,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
        return response;
    }

    public async Task<List<Payer>> GetPage(int currentPage, int pageSize, string query, int payerType = 1)
    {
        var searchQuery = string.IsNullOrEmpty(query) ? "" : query.Trim();
        var linqQuery = _context.Payers
            .Where(p =>
                (p.Name.Contains(searchQuery)
                || p.Email.Contains(searchQuery)
                || p.Phone.Contains(searchQuery)
                || p.BankName.Contains(searchQuery)
                || p.BankNumber.Contains(searchQuery))
                && p.PayerType == payerType && p.Name.Length > 0);
        if (pageSize > 0)
        {
            linqQuery = linqQuery
                .Skip((currentPage) * pageSize)
                .Take(pageSize);
        }
        var data = await linqQuery
            .OrderBy(x => x.Name.ToLower().IndexOf(searchQuery.ToLower()))
            .OrderBy(x => Math.Abs(x.Name.Length - searchQuery.Length))
            .ToListAsync();
        return data ?? new List<Payer>();
    }

    public async Task<int> CountPageTotal(string query = "", int payerType = 1)
    {
        var searchQuery = string.IsNullOrEmpty(query) ? "" : query.Trim();

        return await _context.Payers
            .Where(p =>
                (p.Name.Contains(searchQuery)
                || p.Email.Contains(searchQuery)
                || p.Phone.Contains(searchQuery)
                || p.BankName.Contains(searchQuery)
                || p.BankNumber.Contains(searchQuery))
                && p.PayerType == payerType && p.Name.Length > 0).CountAsync();
    }

    public async Task<string> Create(Payer entity)
    {
        var isExistPayer = _context.Payers.Any(p =>
            (!string.IsNullOrWhiteSpace(entity.Code) && p.Code == entity.Code) || (p.Name == entity.Name && p.Address == entity.Address));
        if (isExistPayer)
        {
            return "Mã đã tồn tại hoặc đồng thời trùng tên và địa chỉ với người dùng có sẵn.";
        }
        CorrectDataForCreate(entity);
        _context.Payers.Add(entity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Update(Payer entity)
    {
        var existingEntity = await _context.Payers.FindAsync(entity.Id);

        if (existingEntity == null)
            return ResultErrorConstants.MODEL_NULL;

        _context.Payers.Update(existingEntity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Delete(long id)
    {
        var currentPayer =
            await _context.Payers.FindAsync(id);
        if (currentPayer == null)
        {
            return ResultErrorConstants.MODEL_NULL;
        }

        _context.Payers.Remove(currentPayer);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    private void CorrectDataForCreate(Payer entity)
    {
        entity.Code = entity.Code.ToUpper();
    }

    public async Task<List<Payer>> GetTaxCodes(int currentPage, int pageSize, string query = "")
    {
        var searchQuery = string.IsNullOrEmpty(query) ? "" : query.Trim();
        if (currentPage == 0)
            currentPage = 1;
        var lquery = _context.Payers
            .Where(p =>
                p.TaxCode.Contains(searchQuery)
                && p.TaxCode.Length > 0
                && p.PayerType == 2);
        if (pageSize > 0)
        {
            lquery = lquery
                       .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }
        var data = await lquery
            .ToListAsync();
        return data ?? new List<Payer>();
    }

    public async Task<int> CountTaxCodeTotal(string query = "")
    {
        var searchQuery = string.IsNullOrEmpty(query) ? "" : query.Trim();
        return await _context.Payers
            .Where(p =>
                p.TaxCode.Contains(searchQuery)
                && p.TaxCode.Length > 0
                && p.PayerType == 2).CountAsync();
    }

    public async Task Delete(IEnumerable<long> ids)
    {
        var entity = await _context.Payers.Where(x => ids.Contains(x.Id)).ToListAsync();
        _context.Payers.RemoveRange(entity);
        await _context.SaveChangesAsync();
    }
}