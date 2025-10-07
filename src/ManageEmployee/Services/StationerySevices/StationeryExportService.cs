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
public class StationeryExportService : IStationeryExportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StationeryExportService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<StationeryExportGetterModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.StationeryExports
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText))
                    .Select(x => _mapper.Map<StationeryExportGetterModel>(x));

        return new PagingResult<StationeryExportGetterModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(StationeryExportModel param)
    {
        var stationery = _mapper.Map<StationeryExport>(param);
        await _context.StationeryExports.AddAsync(stationery);
        await _context.SaveChangesAsync();
        
        var item = param.Items.ConvertAll(x => new StationeryExportItem
        {
            Quantity = x.Quantity,
            StationeryId = x.StationeryId,
            StationeryExportId = stationery.Id
        });
        await _context.StationeryExportItems.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(StationeryExportModel param)
    {
        var stationery = await _context.StationeryExports.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var itemDel = await _context.StationeryExportItems.Where(x => x.StationeryExportId == param.Id).ToListAsync();
        _context.StationeryExportItems.RemoveRange(itemDel);
        var item = param.Items.ConvertAll(x => new StationeryExportItem
        {
            Quantity = x.Quantity,
            StationeryId = x.StationeryId,
            StationeryExportId = stationery.Id
        });
        await _context.StationeryExportItems.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.StationeryExports.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.StationeryExports.Remove(stationery);

        var itemDel = await _context.StationeryExportItems.Where(x => x.StationeryExportId == id).ToListAsync();
        _context.StationeryExportItems.RemoveRange(itemDel);
        await _context.SaveChangesAsync();
    }

    public async Task<StationeryExportModel> GetById(int id)
    {
        var stationery = await _context.StationeryExports.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var import = _mapper.Map<StationeryExportModel>(stationery);
        import.Items = await _context.StationeryExportItems.Where(x => x.StationeryExportId == id)
                    .Select(x => _mapper.Map<StationeryExportItemModel>(x)).ToListAsync();

        return import;
    }
    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.StationeryExports.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
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
