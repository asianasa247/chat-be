using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services.CarServices;
public class DriverRouterService: IDriverRouterService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DriverRouterService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<DriverRouterPagingModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.DriverRouters
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Note.Contains(param.SearchText))
                    .Select(x => _mapper.Map<DriverRouterPagingModel>(x));

        var data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync();
        foreach(var item in data)
        {
            var petrolConsumption = await _context.PetrolConsumptions.FirstOrDefaultAsync(x => x.Id == item.PetrolConsumptionId);
            if (petrolConsumption is null)
            {
                continue;
            }
            item.RoadRouteName = await _context.RoadRoutes.Where(x => x.Id == petrolConsumption.RoadRouteId).Select(x => x.Name).FirstOrDefaultAsync();
            item.LicensePlates = await _context.Cars.Where(x => x.Id == petrolConsumption.CarId).Select(x => x.LicensePlates).FirstOrDefaultAsync();
            item.Driver = await _context.Users.Where(x => x.Id == petrolConsumption.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
            item.KmFrom = petrolConsumption.KmFrom;
            item.KmTo = petrolConsumption.KmTo;
        }
        return new PagingResult<DriverRouterPagingModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = data,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Start(int petrolConsumptionId)
    {
        var driverRouter = await _context.DriverRouters.FirstOrDefaultAsync(x => x.PetrolConsumptionId == petrolConsumptionId && x.Status != nameof(DriverRouterStatus.Finish));
        if (driverRouter == null)
        {
            driverRouter = new DriverRouter
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Date = DateTime.Now,
                PetrolConsumptionId = petrolConsumptionId,
                Status = nameof(DriverRouterStatus.Start),
            };
            driverRouter.AdvancePaymentAmount = await _context.PetrolConsumptions.Where(x => x.Id == petrolConsumptionId).SumAsync(x => x.AdvanceAmount);

            await _context.DriverRouters.AddAsync(driverRouter);
            await _context.SaveChangesAsync();
        }
        //add detail
        var petrolConsumption = await _context.PetrolConsumptions.FindAsync(petrolConsumptionId);
        if (petrolConsumption is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var detail = await _context.DriverRouterDetails.FirstOrDefaultAsync(x => x.DriverRouterId == driverRouter.Id && x.Status == nameof(DriverRouterStatus.Start));
        if (detail is null)
        {
            detail = new DriverRouterDetail
            {
                DriverRouterId = driverRouter.Id,
                Location = petrolConsumption.LocationFrom,
                Date = DateTime.Now,
                Status = nameof(DriverRouterStatus.Start)
            };
            await _context.DriverRouterDetails.AddAsync(detail);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Finish(int petrolConsumptionId)
    {
        var item = await _context.DriverRouters.FirstOrDefaultAsync(x => x.PetrolConsumptionId == petrolConsumptionId);
        item.Status = nameof(DriverRouterStatus.Finish);
        var petrolConsumption = await _context.PetrolConsumptions.FindAsync(petrolConsumptionId);
        if (petrolConsumption is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == petrolConsumption.CarId);
        item.FuelAmount = await _context.PetrolConsumptions.Where(x => x.Id == petrolConsumptionId).SumAsync(x => (x.KmTo - x.KmFrom)*x.PetroPrice * car.FuelAmount/ 100);

        _context.DriverRouters.Update(item);

        //add detail
        var detail = await _context.DriverRouterDetails.FirstOrDefaultAsync(x => x.DriverRouterId == item.Id && x.Status == nameof(DriverRouterStatus.Finish));
        if (detail is null)
        {
            detail = new DriverRouterDetail
            {
                DriverRouterId = item.Id,
                Location = petrolConsumption.LocationTo,
                Date = DateTime.Now,
                Status = nameof(DriverRouterStatus.Finish)
            };
            await _context.DriverRouterDetails.AddAsync(detail);
        }
        await _context.SaveChangesAsync();
    }
    public async Task Update(DriverRouterModel form)
    {
        var item = await _context.DriverRouters.FindAsync(form.Id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        item.PetrolConsumptionId = form.PetrolConsumptionId;
        item.Note = form.Note;
        item.AdvancePaymentAmount = form.AdvancePaymentAmount;
        item.UpdatedAt = DateTime.Now;
        item.Amount = form.Items.Sum(x => x.Amount);
        item.Status = nameof(DriverRouterStatus.InProgress);

        _context.DriverRouters.Update(item);
        await _context.SaveChangesAsync();

        //add detail
        await AddDetail(form.Items, form.Id);
    }

    private async Task AddDetail(List<DriverRouterDetailModel> items, int id)
    {
        var detailDels = await _context.DriverRouterDetails.Where(x => x.DriverRouterId == id).ToListAsync();
        _context.DriverRouterDetails.RemoveRange(detailDels);
        var detailAdds = new List<DriverRouterDetail>();
        foreach(var item in items)
        {
            var detailAdd = new DriverRouterDetail
            {
                DriverRouterId = id,
                Amount = item.Amount,
                Date = item.Date,
                Location = item.Location,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Id = 0,
                PoliceCheckPointId = item.PoliceCheckPointId,
                Note = item.Note,
                Status = nameof(DriverRouterStatus.InProgress),
                FileStr = JsonConvert.SerializeObject(item.Files),
            };
            
            detailAdds.Add(detailAdd);
        }
        await _context.DriverRouterDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var item = await _context.DriverRouters.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var details = await _context.DriverRouterDetails.Where(x => x.DriverRouterId == id).ToListAsync();
        _context.DriverRouterDetails.RemoveRange(details);
        _context.DriverRouters.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<DriverRouterModel> GetById(int id)
    {
        var item = await _context.DriverRouters.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var itemOut = _mapper.Map<DriverRouterModel>(item);
        var details = await _context.DriverRouterDetails.Where(x => x.DriverRouterId == id).ToListAsync();
        itemOut.Items = new List<DriverRouterDetailModel>();
        foreach (var detail in details)
        {
            var detailOut = _mapper.Map<DriverRouterDetailModel>(detail);
            if (!string.IsNullOrEmpty(detail.FileStr))
            {
                detailOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(detail.FileStr);
            }

            itemOut.Items.Add(detailOut);
        }
        
        itemOut.AdvancePaymentAmount = await _context.PetrolConsumptions.Where(x => x.Id == itemOut.PetrolConsumptionId).SumAsync(x => x.AdvanceAmount);
        itemOut.CostAmount = details.Sum(x => x.Amount);
        itemOut.RemainingAmount = (itemOut.AdvancePaymentAmount ?? 0) - (itemOut.FuelAmount ?? 0) - (double)(itemOut.CostAmount ?? 0);

        return itemOut;
    }

    public async Task<IEnumerable<PoliceCheckPointModel>> GetListPoliceCheckPoint(int id)
    {
        var item = await _context.DriverRouters.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var petrolConsumption = await _context.PetrolConsumptions.FirstOrDefaultAsync(x => x.Id == item.PetrolConsumptionId);
        if(petrolConsumption is null)
        {
            return new List<PoliceCheckPointModel>();
        }
        var roadRoute = await _context.RoadRoutes.FirstOrDefaultAsync(x => x.Id == petrolConsumption.RoadRouteId);
        if(roadRoute is null)
        {
            return new List<PoliceCheckPointModel>();
        }
        var policeCheckPointIds = JsonConvert.DeserializeObject<List<int>>(roadRoute.PoliceCheckPointIdStr);
        return await _context.PoliceCheckPoints.Where(x => policeCheckPointIds.Contains(x.Id)).Select(x => new PoliceCheckPointModel
                            {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Amount = x.Amount,
        }).ToListAsync();
    }

}
