using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;
using ManageEmployee.Services.Interfaces.Cultivation;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Cultivation
{
    public class PlantingTypeService : IPlantingTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlantingTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagingResult<PlantingType>> GetAll(int pageIndex, int pageSize, string keyword, int? category = null)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageIndex < 1) pageIndex = 1;

            IQueryable<PlantingType> query = _context.PlantingTypes.AsNoTracking();

            if (category.HasValue)
                query = query.Where(x => (int)x.Category == category.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Code.Contains(keyword) || x.Name.Contains(keyword));

            return new PagingResult<PlantingType>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query
                    .OrderBy(x => x.Category).ThenBy(x => x.Code)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };
        }

        public IEnumerable<PlantingType> GetAll()
            => _context.PlantingTypes.AsNoTracking().OrderBy(x => x.Category).ThenBy(x => x.Code).ToList();

        public async Task<string> Create(PlantingType request)
        {
            var dup = await _context.PlantingTypes
                .AnyAsync(x => x.Category == request.Category && x.Code.ToLower() == request.Code.ToLower());
            if (dup) return "Type_CodeAlreadyExist";

            _context.PlantingTypes.Add(request);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public PlantingType GetById(int id) => _context.PlantingTypes.Find(id)!;

        public async Task<string> Update(PlantingType request)
        {
            var entity = await _context.PlantingTypes.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (entity == null) return "Type_NotFound";

            var dup = await _context.PlantingTypes.AnyAsync(x =>
                x.Id != request.Id && x.Category == request.Category && x.Code.ToLower() == request.Code.ToLower());
            if (dup) return "Type_CodeAlreadyExist";

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Category = request.Category;

            _context.Entry(entity).Property(x => x.Code).IsModified = true;
            _context.Entry(entity).Property(x => x.Name).IsModified = true;
            _context.Entry(entity).Property(x => x.Category).IsModified = true;

            var affected = await _context.SaveChangesAsync();
            return affected > 0 ? string.Empty : "NoChangeDetected";
        }

        public string Delete(int id)
        {
            var entity = _context.PlantingTypes.Find(id);
            if (entity != null)
            {
                _context.PlantingTypes.Remove(entity);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
