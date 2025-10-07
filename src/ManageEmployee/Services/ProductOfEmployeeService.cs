using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Entities.AddressEntities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageEmployee.Services
{
    public class ProductOfEmployeeService : IProductOfEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductOfEmployeeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductOfEmployee>> GetAllAsync()
        {
            return (await _context.ProductOfEmployees.ToListAsync()) ?? new List<ProductOfEmployee>();
        }
        public async Task<IEnumerable<ProductOfEmployee>> GetProductByEmployeeIdAsync(int id)
        {
            return (await _context.ProductOfEmployees.Where(e => e.EmployeeId == id).ToListAsync()) ?? new List<ProductOfEmployee>();
        }
        public async Task<IEnumerable<ProductOfEmployee>> GetProductByCommIdAsync(int id, int employeeId)
        {
            return (await _context.ProductOfEmployees.Where(e => e.CommissionId == employeeId).Where(e => e.EmployeeId == id).ToListAsync()) ?? new List<ProductOfEmployee>();
        }
        public async Task<IEnumerable<ProductOfEmployee>> GetProductByGoodIdAsync(int id)
        {
            return (await _context.ProductOfEmployees.Where(e => e.GoodId == id).ToListAsync()) ?? new List<ProductOfEmployee>();
        }

        public async Task<IEnumerable<ProductOfEmployee>> GetProductByGoodIdAndEmployeeIdAsync(int id,int empId,int commissionId)
        {
            return (await _context.ProductOfEmployees
                .Where(e => e.GoodId == id )
                .Where(e => e.EmployeeId == empId)
                .Where(e => e.CommissionId == commissionId)
                .ToListAsync()) ?? new List<ProductOfEmployee>();
        }

        public async Task<ProductOfEmployee> GetByIdAsync(int id)
        {
            return await _context.ProductOfEmployees.FindAsync(id);
        }

        public async Task<ProductOfEmployee> AddOrUpdateAsync(ProductOfEmployeeModels productModel)
        {
            var productEntity = await _context.ProductOfEmployees
                .FirstOrDefaultAsync(p => p.EmployeeId == productModel.EmployeeId && p.GoodId == productModel.GoodId && p.CommissionId == productModel.CommissionId);

            if (productEntity != null)
            {
                // Nếu đã tồn tại, cập nhật discount
                productEntity.Discount = productModel.Discount;
                _context.ProductOfEmployees.Update(productEntity);
            }
            else
            {
                // Nếu chưa có, tạo mới
                productEntity = _mapper.Map<ProductOfEmployee>(productModel);
                await _context.ProductOfEmployees.AddAsync(productEntity);
            }
            
            await _context.SaveChangesAsync();
            return productEntity;
        }


        public async Task<ProductOfEmployee> UpdateAsync(int id, ProductOfEmployeeModels productModel)
        {
            var existingProduct = await _context.ProductOfEmployees.FindAsync(id);
            _mapper.Map(productModel, existingProduct);
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.ProductOfEmployees.FindAsync(id);
            if (product != null)
            {
                _context.ProductOfEmployees.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
