using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services.ProduceProductServices;
public class CarDeliveryService: ICarDeliveryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CarDeliveryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task Set(CarDeliveryModel model, string tableName, int id)
    {
        var carDelivery = await _context.CarDeliveries.Where(x => x.TableName == tableName && x.TableId == id).FirstOrDefaultAsync();

        if (carDelivery is null)
        {
            carDelivery = new CarDelivery();
        }
        carDelivery.TableId = id;
        carDelivery.TableName = tableName;
        carDelivery.LicensePlates = model.LicensePlates;
        carDelivery.PhoneNumber = model.PhoneNumber;
        carDelivery.Driver = model.Driver;
        carDelivery.Note = model.Note;
        carDelivery.FileLink = JsonConvert.SerializeObject(model.FileLinks);

        _context.CarDeliveries.Update(carDelivery);
        await _context.SaveChangesAsync();
    }

    public async Task ResetTableId(CarDelivery carDelivery, int tableId)
    {
        carDelivery.TableId = tableId;
        _context.CarDeliveries.Update(carDelivery);
        await _context.SaveChangesAsync();
    }

    public async Task Add(CarDelivery carDelivery, string tableName, int id)
    {
        var carDeliveryAdd = new CarDelivery();
        carDeliveryAdd.TableId = id;
        carDeliveryAdd.TableName = tableName;
        carDeliveryAdd.LicensePlates = carDelivery.LicensePlates;
        carDeliveryAdd.PhoneNumber = carDelivery.PhoneNumber;
        carDeliveryAdd.Driver = carDelivery.Driver;
        carDeliveryAdd.Note = carDelivery.Note;
        await _context.CarDeliveries.AddAsync(carDeliveryAdd);
    }

    public async Task<CarDeliveryModel> Get(int? id)
    {
        var data = await _context.CarDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if(data is null)
            return new CarDeliveryModel();
        var itemOut = _mapper.Map<CarDeliveryModel>(data);
        if (!string.IsNullOrEmpty(data.FileLink))
        {
            itemOut.FileLinks = JsonConvert.DeserializeObject<List<FileDetailModel>>(data.FileLink);
        }
        return itemOut;
    }

    public async Task<List<CarDelivery>> GetListForTableName(string tableName, IEnumerable<int> tableIds)
    {
        return await _context.CarDeliveries.Where(x => tableIds.Contains(x.TableId) && x.TableName == tableName).ToListAsync();
    }
}
