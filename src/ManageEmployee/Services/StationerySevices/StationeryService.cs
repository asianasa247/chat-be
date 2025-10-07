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
public class StationeryService : IStationeryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public StationeryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<StationeryModel>> GetList()
    {
        return await _context.Stationeries.Select(x => _mapper.Map<StationeryModel>(x)).ToListAsync();
    }
    public async Task<PagingResult<StationeryModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.Stationeries
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Code.Contains(param.SearchText) || x.Name.Contains(param.SearchText))
                    .Select(x => _mapper.Map<StationeryModel>(x));

        return new PagingResult<StationeryModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(StationeryModel param)
    {
        var stationery = _mapper.Map<Stationery>(param);
        await _context.Stationeries.AddAsync(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task Update(StationeryModel param)
    {
        var stationery = await _context.Stationeries.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        stationery.Code = param.Code;
        stationery.Name = param.Name;
        stationery.Unit = param.Unit;
        stationery.UpdatedAt = DateTime.Now;

        _context.Stationeries.Update(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.Stationeries.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.Stationeries.Remove(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task<StationeryModel> GetById(int id)
    {
        var stationery = await _context.Stationeries.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        return _mapper.Map<StationeryModel>(stationery);
    }
}
