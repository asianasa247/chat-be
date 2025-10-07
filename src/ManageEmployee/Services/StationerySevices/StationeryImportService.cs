using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Stationery;
using ManageEmployee.Entities.StationeryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Stationeries;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.StationerySevices;
public class StationeryImportService : IStationeryImportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StationeryImportService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<StationeryImportGetterModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.StationeryImports
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText))
                    .Select(x => _mapper.Map<StationeryImportGetterModel>(x));

        var stationeryImports = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync();
        var stationeryImportIds = stationeryImports.Select(x => x.Id);
        var stationeryImportItems = await _context.StationeryImportItems.Where(x => stationeryImportIds.Contains(x.StationeryImportId)).ToListAsync();
        stationeryImports = stationeryImports.ConvertAll(s =>
        {
            s.Quantity = stationeryImportItems.Where(x => x.StationeryImportId == s.Id).Sum(x => x.Quantity);
            s.TotalAmount = stationeryImportItems.Where(x => x.StationeryImportId == s.Id).Sum(x => (x.UnitPrice  ?? 0)* x.Quantity);
            return s;
        });

        return new PagingResult<StationeryImportGetterModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = stationeryImports,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(StationeryImportModel param)
    {
        var stationery = _mapper.Map<StationeryImport>(param);
        await _context.StationeryImports.AddAsync(stationery);
        await _context.SaveChangesAsync();
        
        var item = param.Items.ConvertAll(x => new StationeryImportItem
        {
            Quantity = x.Quantity,
            StationeryId = x.StationeryId,
            UnitPrice = x.UnitPrice,
            StationeryImportId = stationery.Id
        });
        await _context.StationeryImportItems.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(StationeryImportModel param)
    {
        var stationery = await _context.StationeryImports.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var itemDel = await _context.StationeryImportItems.Where(x => x.StationeryImportId == param.Id).ToListAsync();
        _context.StationeryImportItems.RemoveRange(itemDel);
        var item = param.Items.ConvertAll(x => new StationeryImportItem
        {
            Quantity = x.Quantity,
            StationeryId = x.StationeryId,
            UnitPrice = x.UnitPrice,
            StationeryImportId = stationery.Id
        });
        await _context.StationeryImportItems.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.StationeryImports.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.StationeryImports.Remove(stationery);

        var itemDel = await _context.StationeryImportItems.Where(x => x.StationeryImportId == id).ToListAsync();
        _context.StationeryImportItems.RemoveRange(itemDel);
        await _context.SaveChangesAsync();
    }

    public async Task<StationeryImportModel> GetById(int id)
    {
        var stationery = await _context.StationeryImports.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var import = _mapper.Map<StationeryImportModel>(stationery);
        import.Items = await _context.StationeryImportItems.Where(x => x.StationeryImportId == id)
                    .Select(x => _mapper.Map<StationeryImportItemModel>(x)).ToListAsync();

        return import;
    }
    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.StationeryImports.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        if (item == null)
            return "0000001";
        try
        {

            int.TryParse(item.ProcedureNumber, out int order);
            var procedureNumber = order.ToString();
            while (true)
            {
                if (procedureNumber.Length > 7)
                {
                    break;
                }
                procedureNumber = "0" + procedureNumber;
            }
            return $"{procedureNumber}";
        }
        catch
        {
            return"0000001";
        }
    }
}
