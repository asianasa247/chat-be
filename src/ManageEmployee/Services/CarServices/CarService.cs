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

public class CarService : ICarService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CarService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CarGetterModel>> GetList()
    {
        return await _context.Cars.Select(x => _mapper.Map<CarGetterModel>(x)).ToListAsync();
    }

    public async Task<PagingResult<CarGetterPagingModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.Cars
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.LicensePlates.Contains(param.SearchText) || x.Note.Contains(param.SearchText))
                    .Select(x => _mapper.Map<CarGetterPagingModel>(x));

        return new PagingResult<CarGetterPagingModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(CarModel param)
    {
        var stationery = _mapper.Map<Car>(param);
        if (param.Files != null)
        {
            stationery.FileLink = JsonConvert.SerializeObject(param.Files);
        }
        await _context.Cars.AddAsync(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task Update(CarModel param)
    {
        var stationery = await _context.Cars.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        stationery.LicensePlates = param.LicensePlates;
        stationery.Note = param.Note;
        stationery.Content = param.Content;
        stationery.MileageAllowance = param.MileageAllowance;
        stationery.FuelAmount = param.FuelAmount;
        stationery.UpdatedAt = DateTime.Now;
        if (param.Files != null)
        {
            stationery.FileLink = JsonConvert.SerializeObject(param.Files);
        }

        _context.Cars.Update(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.Cars.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.Cars.Remove(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task<CarGetterDetailModel> GetById(int id)
    {
        var stationery = await _context.Cars.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var itemOut = _mapper.Map<CarGetterDetailModel>(stationery);
        if (!string.IsNullOrEmpty(stationery.FileLink))
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(stationery.FileLink);
        }

        return itemOut;
    }

    public async Task<List<CarFieldSetupGetterModel>> GetCarFieldSetup(int carId)
    {
        var carFields = await _context.CarFields.Where(x => x.CarId == carId).ToListAsync();
        var carfieldSetups = await _context.CarFieldSetups.Where(x => x.CarId == carId).ToListAsync();
        var carFieldSetupModels = new List<CarFieldSetupGetterModel>();

        foreach (var carField in carFields)
        {
            CarFieldSetupGetterModel carfielsSetupModel = new CarFieldSetupGetterModel();
            var carfielsSetup = carfieldSetups.Find(x => x.CarFieldId == carField.Id);
            if (carfielsSetup != null)
            {
                carfielsSetupModel = _mapper.Map<CarFieldSetupGetterModel>(carfielsSetup);
                carfielsSetupModel.UserIds = carfielsSetup.UserIdString?.Split(",").Select(x => Convert.ToInt32(x)).ToList();
                if (carfielsSetup.FileLink != null)
                    carfielsSetupModel.File = JsonConvert.DeserializeObject<FileDetailModel>(carfielsSetup.FileLink);
            }
            carfielsSetupModel.CarFieldName = carField.Name;
            carfielsSetupModel.CarFieldId = carField.Id;

            carFieldSetupModels.Add(carfielsSetupModel);
        }
        return carFieldSetupModels;
    }

    public async Task UpdateCarFieldSetup(int carId, List<CarFieldSetupModel> carFieldSetups)
    {
        var carfieldSetupDels = await _context.CarFieldSetups.Where(x => x.CarId == carId).ToListAsync();
        _context.CarFieldSetups.RemoveRange(carfieldSetupDels);

        var carFieldSetupAdds = new List<CarFieldSetup>();

        foreach (var carFieldSetup in carFieldSetups)
        {
            var carFieldSetupAdd = _mapper.Map<CarFieldSetup>(carFieldSetup);
            carFieldSetupAdd.Id = 0;
            carFieldSetupAdd.CarId = carId;
            carFieldSetupAdd.CreatedAt = DateTime.Now;
            carFieldSetupAdd.UpdatedAt = DateTime.Now;

            if (carFieldSetup.UserIds != null)
            {
                carFieldSetupAdd.UserIdString = string.Join(",", carFieldSetup.UserIds);
            }

            if (carFieldSetup.File != null)
            {
                carFieldSetupAdd.FileLink = JsonConvert.SerializeObject(carFieldSetup.File).ToString();
            }

            carFieldSetupAdds.Add(carFieldSetupAdd);
        }
       
        await _context.CarFieldSetups.AddRangeAsync(carFieldSetupAdds);
        await _context.SaveChangesAsync();
    }
}