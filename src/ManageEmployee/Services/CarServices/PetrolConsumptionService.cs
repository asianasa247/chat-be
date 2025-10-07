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

public class PetrolConsumptionService : IPetrolConsumptionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PetrolConsumptionService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<PetrolConsumptionGetterModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.PetrolConsumptions
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Note.Contains(param.SearchText))
                    .Select(x => _mapper.Map<PetrolConsumptionGetterModel>(x));

        return new PagingResult<PetrolConsumptionGetterModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(PetrolConsumptionModel param)
    {
        var stationery = _mapper.Map<PetrolConsumption>(param);
        await _context.PetrolConsumptions.AddAsync(stationery);
        await _context.SaveChangesAsync();

        if (param.Points != null)
        {
            var points = param.Points.ConvertAll(x => new PetrolConsumptionPoliceCheckPoint
            {
                PetrolConsumptionId = stationery.Id,
                Amount = x.Amount,
                PoliceCheckPointName = x.PoliceCheckPointName,
                IsArise = x.IsArise,
            });
            await _context.PetrolConsumptionPoliceCheckPoints.AddRangeAsync(points);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Update(PetrolConsumptionModel param)
    {
        var petrolConsumption = await _context.PetrolConsumptions.FindAsync(param.Id);
        if (petrolConsumption is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        petrolConsumption.KmFrom = param.KmFrom;
        petrolConsumption.KmTo = param.KmTo;
        petrolConsumption.Note = param.Note;
        petrolConsumption.LocationFrom = param.LocationFrom;
        petrolConsumption.LocationTo = param.LocationTo;
        petrolConsumption.AdvanceAmount = param.AdvanceAmount;
        petrolConsumption.CarId = param.CarId;
        petrolConsumption.Date = param.Date;
        petrolConsumption.PetroPrice = param.PetroPrice;
        petrolConsumption.UserId = param.UserId;

        petrolConsumption.UpdatedAt = DateTime.Now;
        petrolConsumption.RoadRouteId = param.RoadRouteId;

        _context.PetrolConsumptions.Update(petrolConsumption);

        var pointDels = await _context.PetrolConsumptionPoliceCheckPoints.Where(x => x.PetrolConsumptionId == param.Id).ToListAsync();
        _context.PetrolConsumptionPoliceCheckPoints.RemoveRange(pointDels);
        if (param.Points != null)
        {
            var points = param.Points?.ConvertAll(x => new PetrolConsumptionPoliceCheckPoint
            {
                PetrolConsumptionId = petrolConsumption.Id,
                Amount = x.Amount,
                PoliceCheckPointName = x.PoliceCheckPointName,
                IsArise = x.IsArise,
            });
            await _context.PetrolConsumptionPoliceCheckPoints.AddRangeAsync(points);
        }
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.PetrolConsumptions.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.PetrolConsumptions.Remove(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task<PetrolConsumptionModel> GetById(int id)
    {
        var stationery = await _context.PetrolConsumptions.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        return _mapper.Map<PetrolConsumptionModel>(stationery);
    }

    public async Task<IEnumerable<PetrolConsumptionReportModel>> ReportAsync(PetrolConsumptionReportRequestModel param)
    {
        var fromAt = new DateTime(param.FromAt.Year, param.FromAt.Month, param.FromAt.Day);
        var toAt = new DateTime(param.ToAt.Year, param.ToAt.Month, param.ToAt.Day);
        toAt = toAt.AddDays(1);
        var petrolConsumptions = await _context.PetrolConsumptions.Where(x => x.Date >= fromAt && x.Date < toAt && x.CarId == param.CarId).ToListAsync();

        var petrolConsumptionIds = petrolConsumptions.Select(x => x.Id);
        var driverRouters = await _context.DriverRouters.Where(x => x.Date >= fromAt && x.Date < toAt && petrolConsumptionIds.Contains(x.PetrolConsumptionId) ).ToListAsync();

        var userIds = petrolConsumptions.Select(x => x.UserId);
        var users = await _context.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();

        var itemOuts = new List<PetrolConsumptionReportModel>();
        foreach (var petrolConsumption in petrolConsumptions)
        {
            var itemOut = new PetrolConsumptionReportModel
            {
                Id = petrolConsumption.Id,
                Date = petrolConsumption.Date,
                AdvanceAmount = petrolConsumption.AdvanceAmount,
                PetroPrice = petrolConsumption.PetroPrice,
                KmFrom = petrolConsumption.KmFrom,
                KmTo = petrolConsumption.KmTo,
                ExplainNote = petrolConsumption.Note,
                Month = petrolConsumption.Date.Month
            };
            itemOut.UserName = users.FirstOrDefault(x => x.Id == petrolConsumption.UserId)?.FullName;
            itemOut.DateTo = driverRouters.OrderByDescending(x => x.Date).FirstOrDefault()?.Date;
            itemOut.ExpenseAmount = driverRouters.Where(x => x.PetrolConsumptionId == petrolConsumption.Id).Sum(x => x.Amount);
            itemOut.RemainingAmount = Convert.ToDecimal(itemOut.AdvanceAmount) - itemOut.ExpenseAmount;
            var driverRouterIds = driverRouters.Select(x => x.Id);
            var fileStrs = await _context.DriverRouterDetails.Where(x => driverRouterIds.Contains( x.DriverRouterId))
                .Where( x => !string.IsNullOrEmpty(x.FileStr)).Select(x => x.FileStr).ToListAsync();
            itemOut.Files = new List<FileDetailModel>();
            foreach (var fileStr in fileStrs)
            {
                if (fileStr != "null")
                {
                    itemOut.Files.AddRange(JsonConvert.DeserializeObject<List<FileDetailModel>>(fileStr));
                }
            }
            itemOuts.Add(itemOut);
        }
        return itemOuts;
    }
}