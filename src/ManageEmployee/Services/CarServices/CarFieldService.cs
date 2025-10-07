using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Cars;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CarEntities;

namespace ManageEmployee.Services.CarServices;
public class CarFieldService: ICarFieldService
{
    private readonly ApplicationDbContext _context;

    public CarFieldService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<CarFieldPagingModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.CarFields
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText));

        var datas = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync();
        var carIds = datas.Select(X => X.CarId).Distinct();
        var cars = await _context.Cars.Where(x => carIds.Contains(x.Id)).ToListAsync();
             
        var itemOuts = new List<CarFieldPagingModel>();
        foreach(var data in datas)
        {
            var car = cars.Find(x => x.Id == data.CarId);
            itemOuts.Add(new CarFieldPagingModel
            {
                Id = data.Id,
                Name = data.Name,
                Order = data.Order,
                LicensePlates = car?.LicensePlates
            });
        }
        return new PagingResult<CarFieldPagingModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = itemOuts,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(CarFieldModel param)
    {
        var carField = new CarField
        {
            Name=param.Name,
            CarId=param.CarId,
            Order = param.Order,
        };
       
        await _context.CarFields.AddAsync(carField);
        await _context.SaveChangesAsync();
    }

    public async Task Update(CarFieldModel param)
    {
        var carField = await _context.CarFields.FindAsync(param.Id);
        if (carField is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        carField.Name = param.Name;
        carField.CarId = param.CarId;
        carField.Order = param.Order;
        
        _context.CarFields.Update(carField);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var carField = await _context.CarFields.FindAsync(id);
        if (carField is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.CarFields.Remove(carField);
        await _context.SaveChangesAsync();
    }

    public async Task<CarFieldModel> GetById(int id)
    {
        var carField = await _context.CarFields.FindAsync(id);
        if (carField is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        return new CarFieldModel
        {
            Id = carField.Id,
            Name = carField.Name,
            CarId = carField.CarId,
            Order = carField.Order
        };
    }
}
