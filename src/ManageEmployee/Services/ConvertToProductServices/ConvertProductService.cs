using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ConvertProductModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ConvertProductEntities;
using ManageEmployee.Services.Interfaces.ConvertToProduct;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ConvertToProductServices
{
    public class ConvertProductService : IConvertProductService
    {
        private readonly ApplicationDbContext _dbContext;

        public ConvertProductService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ConvertProduct> Add(DTOConvert model)
        {
            var item = new ConvertProduct
            {
                Quantity = model.Quantity,
                ConvertQuantity = model.ConvertQuantity,
                Account = model.Account,
                AccountName = model.AccountName,
                Warehouse = model.Warehouse,
                WarehouseName = model.WarehouseName,
                Detail1 = model.Detail1,
                DetailName1 = model.DetailName1,
                Detail2 = model.Detail2,
                DetailName2 = model.DetailName2,
                OppositeAccount = model.OppositeAccount,
                OppositeAccountName = model.OppositeAccountName,
                OppositeWarehouse = model.OppositeWarehouse,
                OppositeWarehouseName = model.OppositeWarehouseName,
                OppositeDetail1 = model.OppositeDetail1,
                OppositeDetailName1 = model.OppositeDetailName1,
                OppositeDetail2 = model.OppositeDetail2,
                OppositeDetailName2 = model.OppositeDetailName2
            };

            _dbContext.Add(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<ConvertProduct> Update(int id, DTOConvert model)
        {
            var item = await _dbContext.ConvertProducts.FindAsync(id);
            if (item == null) return null;

            item.Quantity = model.Quantity;
            item.ConvertQuantity = model.ConvertQuantity;
            item.Account = model.Account;
            item.AccountName = model.AccountName;
            item.Warehouse = model.Warehouse;
            item.WarehouseName = model.WarehouseName;
            item.Detail1 = model.Detail1;
            item.DetailName1 = model.DetailName1;
            item.Detail2 = model.Detail2;
            item.DetailName2 = model.DetailName2;
            item.OppositeAccount = model.OppositeAccount;
            item.OppositeAccountName = model.OppositeAccountName;
            item.OppositeWarehouse = model.OppositeWarehouse;
            item.OppositeWarehouseName = model.OppositeWarehouseName;
            item.OppositeDetail1 = model.OppositeDetail1;
            item.OppositeDetailName1 = model.OppositeDetailName1;
            item.OppositeDetail2 = model.OppositeDetail2;
            item.OppositeDetailName2 = model.OppositeDetailName2;

            _dbContext.Update(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<bool> Delete(int id)
        {
            var item = await _dbContext.ConvertProducts.FindAsync(id);
            if (item == null) return false;

            _dbContext.ConvertProducts.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ConvertProduct> GetById(int id)
        {
            return await _dbContext.ConvertProducts.FindAsync(id);
        }

        public async Task<object> GetAll(PagingRequestModel request)
        {
            if (request.Page < 1 || request.PageSize < 1)
            {
                return null;
            }

            var query = _dbContext.ConvertProducts.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(item =>
                    item.DetailName1.Contains(request.SearchText) ||
                    item.DetailName2.Contains(request.SearchText));
            }

            if (request.isSort && !string.IsNullOrEmpty(request.SortField))
            {
                query = request.SortField.ToLower() switch
                {
                    "detailname1" => query.OrderBy(item => item.DetailName1),
                    "detailname2" => query.OrderBy(item => item.DetailName2),
                    _ => query
                };
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Data = items
            };
        }
    }

}
