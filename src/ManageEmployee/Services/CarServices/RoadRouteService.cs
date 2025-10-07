using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services.CarServices;
public class RoadRouteService: IRoadRouteService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public RoadRouteService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task Create(RoadRouteModel form)
    {
        var item = _mapper.Map<RoadRoute>(form);
        item.PoliceCheckPointIdStr = JsonConvert.SerializeObject(form.PoliceCheckPointIds);
        await _context.RoadRoutes.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(RoadRouteModel form)
    {
        var item = _mapper.Map<RoadRoute>(form);
        item.PoliceCheckPointIdStr = JsonConvert.SerializeObject(form.PoliceCheckPointIds);
        _context.RoadRoutes.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task<RoadRouteModel> GetDetail(int id)
    {
        var item = await _context.RoadRoutes.FindAsync(id);
        var itemOut = _mapper.Map<RoadRouteModel>(item);
        if (!string.IsNullOrEmpty(item.PoliceCheckPointIdStr))
        {
            itemOut.PoliceCheckPointIds = JsonConvert.DeserializeObject<List<int>>(item.PoliceCheckPointIdStr);
        }
        return itemOut;
    }

    public async Task<PagingResult<RoadRoutePagingModel>> GetPaging(PagingRequestModel searchRequest)
    {
        var query = _context.RoadRoutes.Where(x => string.IsNullOrEmpty(searchRequest.SearchText) || x.Name.Contains(searchRequest.SearchText));

        var data = await query.Skip(searchRequest.Page * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync();
        var itemOuts = new List<RoadRoutePagingModel>();

        foreach(var item in data)
        {
            var itemOut = _mapper.Map<RoadRoutePagingModel>(item);

            if (!string.IsNullOrEmpty(item.PoliceCheckPointIdStr))
            {
                var policeCheckPointIds = JsonConvert.DeserializeObject<List<int>>(item.PoliceCheckPointIdStr);
                itemOut.PoliceCheckPoint = String.Join("; ", await _context.PoliceCheckPoints.Where(x => policeCheckPointIds.Contains(x.Id)).Select(x => x.Name).ToListAsync());
            }
            itemOuts.Add(itemOut);
        }
        return new PagingResult<RoadRoutePagingModel>
        {
            Data = itemOuts,
            CurrentPage = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalItems = await query.CountAsync()
        };
    }
    public async Task Delete(int id)
    {

        var item = await _context.RoadRoutes.FindAsync(id);
        if(item is null)
        {
            throw new ErrorException(ErrorMessages.DataExist);
        }
        _context.RoadRoutes.Remove(item);
        await _context.SaveChangesAsync();
    }

}
