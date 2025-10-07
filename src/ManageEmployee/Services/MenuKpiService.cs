using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services;
public class MenuKpiService : IMenuKpiService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MenuKpiService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<MenuKpi> GetAll()
    {

        var data = _context.MenuKpis.Select(x => new MenuKpi
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            FromValue = x.FromValue,
            ToValue = x.ToValue,
            Point = x.Point,
        }).ToList();
        return data;
    }

    public async Task<PagingResult<MenuKpi>> GetAll(int pageIndex, int pageSize, string keyword, int type)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 0;
           
            var datas =  _context.MenuKpis
                .Where(x => x.Type == type)
                .Select(x => new MenuKpi
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                FromValue = x.FromValue,
                ToValue = x.ToValue,
                Point = x.Point,
                Note = x.Note,
                Type = x.Type,
            });

            var result = new PagingResult<MenuKpi>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Data = await datas.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                TotalItems = await datas.CountAsync()
            };
            return result;
        }
        catch
        {
            return new PagingResult<MenuKpi>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<MenuKpi>()
            };
        }
    }

    public async Task<string> Create(MenuKpi request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var existCode = _context.MenuKpis.Where(
                x => x.Code.ToLower() == request.Code.ToLower()).FirstOrDefault();
            if (existCode != null)
            {
                return ErrorMessages.MenuKpiCodeAlreadyExist;
            }
            MenuKpi MenuKpi = _mapper.Map<MenuKpi>(request);
            _context.MenuKpis.Add(MenuKpi);
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

    public MenuKpi GetById(int id)
    {
        var MenuKpi = _context.MenuKpis.Find(id);
        return MenuKpi;
    }

    public async Task<string> Update(MenuKpi request)
    {
        var checkMenuKpiCode = await _context.MenuKpis.Where(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != request.Id).FirstOrDefaultAsync();
        if (checkMenuKpiCode != null && checkMenuKpiCode.Id != request.Id)
        {
            return ErrorMessages.MenuKpiCodeAlreadyExist;
        }
        _context.MenuKpis.Update(request);

        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public string Delete(int id)
    {
        var MenuKpi = _context.MenuKpis.Find(id);
        if (MenuKpi != null)
        {
            _context.MenuKpis.Remove(MenuKpi);
            _context.SaveChanges();
        }
        return string.Empty;
    }
}
