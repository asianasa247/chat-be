using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Inventorys;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services;
public class InvoiceDeclarationService : IInvoiceDeclarationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public InvoiceDeclarationService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<string> Create(InvoiceDeclarationModel request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            var entity = new InvoiceDeclaration();
            entity.Name = request.Name;
            entity.InvoiceSymbol = request.InvoiceSymbol;
            entity.TemplateSymbol = request.TemplateSymbol;
            entity.ToOpening = request.ToOpening;
            entity.FromOpening = request.FromOpening;
            entity.ToArising = request.ToArising;
            entity.FromArising = request.FromArising;
            entity.Note = request.Note;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            _context.InvoiceDeclarations.Add(entity);
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

    public async Task Delete(int id)
    {
        var itemDelete = await _context.InvoiceDeclarations.FindAsync(id);
        if (itemDelete != null)
        {
            _context.InvoiceDeclarations.Remove(itemDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<InvoiceDeclarationModel>> GetAll()
    { 
        var data = await _context.InvoiceDeclarations.Where(x => !x.IsDelete).Select(x => new InvoiceDeclarationModel
        {
            Id = x.Id,
            Name = x.Name,
            InvoiceSymbol = x.InvoiceSymbol,
            TemplateSymbol = x.TemplateSymbol,
            ToOpening = x.ToOpening,
            FromOpening = x.FromOpening,
            ToArising = x.ToArising,
            FromArising = x.FromArising,
            Note = x.Note,
        }).ToListAsync();
        return data;
    }

    public async Task<PagingResult<InvoiceDeclarationModel>> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var InvoiceDeclarations = await _context.InvoiceDeclarations.Where(x => x.IsDelete != true && x.Id != 0)
                                         .Where(x => string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(keyword.ToLower())))
                                         .Select(x => new InvoiceDeclarationModel
                                         {
                                             Id = x.Id,
                                             Name = x.Name,
                                             InvoiceSymbol = x.InvoiceSymbol,
                                             TemplateSymbol = x.TemplateSymbol,
                                             ToOpening = x.ToOpening,
                                             FromOpening = x.FromOpening,
                                             ToArising = x.ToArising,
                                             FromArising = x.FromArising,
                                             Note = x.Note,
                                             FromUsed = x.FromUsed,
                                             ToUsed = x.ToUsed,
                                             TotalUsed = x.ToUsed > 0 ? ( x.ToUsed - x.FromUsed +1 ) : 0,
                                             UsedNumber = x.UsedNumber,
                                             DeleteNumber = x.DeleteNumber,
                                             DeleteNumberItem = x.DeleteNumberItem,
                                             FromClosing = x.FromOpening + ( x.Month == 0 ? x.FromOpening : (x.ToUsed > 0 ? (x.ToUsed + 1) : 0)),
                                             Month = x.Month
                                         })
                                         .ToListAsync();
            InvoiceDeclarations = InvoiceDeclarations.ConvertAll(x => {
                x.TotalRelease = TotalReleaseInvoiceInMonth(x);
                x.FromClosing = x.FromOpening  + x.TotalUsed;
                x.ToClosing = x.TotalRelease + x.FromClosing - 1;
                x.ClosingNumber = x.ToClosing - x.FromClosing + 1;
                return x;
            });

            return new PagingResult<InvoiceDeclarationModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = InvoiceDeclarations.Count,
                Data = InvoiceDeclarations.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
        }
        catch
        {
            return new PagingResult<InvoiceDeclarationModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<InvoiceDeclarationModel>()
            };
        }
    }

    public InvoiceDeclarationModel GetById(int id)
    {
        try
        {
            var itemData = _context.InvoiceDeclarations.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefault();
            if (itemData != null)
            {
                return new InvoiceDeclarationModel
                {
                    Id = itemData.Id,
                    Name = itemData.Name,
                    InvoiceSymbol = itemData.InvoiceSymbol,
                    TemplateSymbol = itemData.TemplateSymbol,
                    ToOpening = itemData.ToOpening,
                    FromOpening = itemData.FromOpening,
                    ToArising = itemData.ToArising,
                    FromArising = itemData.FromArising,
                    Note = itemData.Note,
                };
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

    public async Task<InvoiceDeclarationModel> Update(InvoiceDeclarationModel request)
    {
        try
        {
            var entity = _context.InvoiceDeclarations.Find(request.Id);
            if (entity == null)
            {
                throw new Exception("");
            }
            entity.Name = request.Name;
            entity.InvoiceSymbol = request.InvoiceSymbol;
            entity.TemplateSymbol = request.TemplateSymbol;
            entity.ToOpening = request.ToOpening;
            entity.FromOpening = request.FromOpening;
            entity.ToArising = request.ToArising;
            entity.FromArising = request.FromArising;
            entity.Note = request.Note;


            _context.InvoiceDeclarations.Update(entity);
            await _context.SaveChangesAsync();

            InvoiceDeclarationModel item = MapClosing(entity);
            return item;
        }
        catch
        {
            throw;
        }
    }
    public async Task<InvoiceDeclarationModel> UpdateInvoice(int id, int year)
    {
        try
        {
            var entity = _context.InvoiceDeclarations.Find(id);
            if (entity == null)
            {
                throw new Exception("");
            }
            if(entity.Month > 0)
            {
                var entityBefore = _context.InvoiceDeclarations.FirstOrDefault(x => x.Month == entity.Month - 1);

                var itemMapData = MapClosing(entityBefore);
                entity.FromOpening = itemMapData.FromClosing;
                entity.ToOpening = itemMapData.ToClosing;
            }
           

            var ledgers = _context.GetLedger(year).Where(x => x.InvoiceCode == entity.Name && x.Month == entity.Month 
                                && x.InvoiceNumber != null && x.InvoiceNumber.Length == 7).ToList();
            if (ledgers.Count > 0)
            {

                entity.FromUsed = ledgers.Min(x => int.Parse(x.InvoiceNumber ?? "0"));
                entity.ToUsed = ledgers.Max(x => int.Parse(x.InvoiceNumber ?? "0"));
                entity.UsedNumber = ledgers.Select(x => x.InvoiceNumber).Distinct().Count();
                entity.DeleteNumber = ledgers.Where(x => x.InvoiceAdditionalDeclarationCode == "HB").Select(x => x.InvoiceNumber).Distinct().Count();
                entity.DeleteNumberItem = string.Join(";", ledgers.Where(x => x.InvoiceAdditionalDeclarationCode == "HB").Select(x => x.InvoiceNumber).Distinct());
            }
            else
            {
                entity.FromUsed = 0;
                entity.ToUsed = 0;
                entity.UsedNumber = 0;
                entity.DeleteNumber = 0;
                entity.DeleteNumberItem = "";
            }
            
            _context.InvoiceDeclarations.Update(entity);
            await _context.SaveChangesAsync();

            InvoiceDeclarationModel item = MapClosing(entity);
            return item;
        }
        catch
        {
            throw;
        }
    }

    public async Task<InvoiceDeclarationModel> ResetInvoice(int id)
    {
        var invoice = await _context.InvoiceDeclarations.FindAsync(id);
        if(invoice is null)
        {
            throw new Exception("");
        }
        invoice.FromOpening = 0;
        invoice.ToOpening = 0;
        invoice.FromArising = 0;
        invoice.ToArising = 0;

        _context.InvoiceDeclarations.Update(invoice);
        await _context.SaveChangesAsync();

        var item = MapClosing(invoice);
        return item;
    }
    private InvoiceDeclarationModel MapClosing(InvoiceDeclaration entity)
    {
        InvoiceDeclarationModel item = _mapper.Map<InvoiceDeclarationModel>(entity);
        item.TotalRelease = TotalReleaseInvoiceInMonth(item);
        item.TotalUsed = entity.ToUsed > 0 ? (entity.ToUsed - entity.FromUsed + 1) : 0;

        int totalArsing = TotalArsingInvoiceInMonth(entity.FromArising ?? 0, entity.ToArising ?? 0);
        item.FromClosing = entity.FromOpening + item.TotalUsed;
        item.ToClosing =  entity.ToOpening + totalArsing;

        item.ClosingNumber = item.ToClosing - (item.FromClosing > 0 ? (item.FromClosing - 1) : 0);
        return item;
    }
    private int TotalReleaseInvoiceInMonth(InvoiceDeclarationModel entity)
    {
        int totalOpening = (entity.ToOpening ?? 0) - (entity.FromOpening ?? 0);
        if (totalOpening > 0)
            totalOpening += 1;

        int totalArsing = TotalArsingInvoiceInMonth(entity.FromArising ?? 0, entity.ToArising ?? 0);

        return totalOpening + totalArsing;
    }
    private int TotalArsingInvoiceInMonth(int fromArising, int toArising)
    {
        int totalArsing = toArising - fromArising;
        if (totalArsing > 0)
            totalArsing += 1;

        return totalArsing;
    }
}
