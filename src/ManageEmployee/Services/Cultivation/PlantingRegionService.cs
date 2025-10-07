using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;
using ManageEmployee.Services.Interfaces.Cultivation;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Cultivation
{
    public class PlantingRegionService : IPlantingRegionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlantingRegionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagingResult<PlantingRegion>> GetAll(int page, int pageSize, string keyword, int? countryId = null, int? typeId = null)
        {
            if (pageSize <= 0) pageSize = 20;
            if (page < 1) page = 1;

            IQueryable<PlantingRegion> query = _context.PlantingRegions.AsNoTracking();

            if (countryId.HasValue) query = query.Where(x => x.CountryId == countryId.Value);
            if (typeId.HasValue) query = query.Where(x => x.TypeId == typeId.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Code.Contains(keyword) ||
                    x.Name.Contains(keyword) ||
                    (x.Address != null && x.Address.Contains(keyword)) ||
                    (x.Manager != null && x.Manager.Contains(keyword)) ||
                    x.Type.Name.Contains(keyword) ||
                    x.Type.Code.Contains(keyword));
            }

            var result = new PagingResult<PlantingRegion>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query
                    .Include(x => x.Type)
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };
            return result;
        }

        public IEnumerable<PlantingRegion> GetAll()
        {
            return _context.PlantingRegions
                .AsNoTracking()
                .Include(x => x.Type)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public async Task<string> Create(PlantingRegion request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                // validate Type (tồn tại)
                var type = await _context.PlantingTypes.FindAsync(request.TypeId);
                if (type == null) return "TypeNotFound";

                // unique (CountryId, Code)
                var dup = await _context.PlantingRegions.AnyAsync(x =>
                    x.CountryId == request.CountryId &&
                    x.Code.ToLower() == request.Code.ToLower());
                if (dup) return "Region_CodeAlreadyExist";

                var entity = _mapper.Map<PlantingRegion>(request);
                _context.PlantingRegions.Add(entity);
                await _context.SaveChangesAsync();

                await _context.Database.CommitTransactionAsync();
                return string.Empty;
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public PlantingRegion GetById(int id) => _context.PlantingRegions.Find(id)!;

        public async Task<string> Update(PlantingRegion request)
        {
            try
            {
                request.Type = null; // tránh EF insert Type

                var entity = await _context.PlantingRegions
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (entity == null) return "Region_NotFound";

                // Type phải tồn tại
                var typeExists = await _context.PlantingTypes.AnyAsync(t => t.Id == request.TypeId);
                if (!typeExists) return "TypeNotFound";

                // Unique (CountryId, Code) trừ bản ghi hiện tại
                var dup = await _context.PlantingRegions.AnyAsync(x =>
                    x.Id != request.Id &&
                    x.CountryId == request.CountryId &&
                    x.Code.ToLower() == request.Code.ToLower());
                if (dup) return "Region_CodeAlreadyExist";

                // Patch tất cả field có thể sửa
                entity.CountryId = request.CountryId;
                entity.Code = request.Code;
                entity.Name = request.Name;
                entity.Note = request.Note;
                entity.Latitude = request.Latitude;
                entity.Longitude = request.Longitude;
                entity.Manager = request.Manager;
                entity.Quantity = request.Quantity;
                entity.TypeId = request.TypeId;
                entity.Area = request.Area;
                entity.StartDate = request.StartDate;
                entity.HarvestDate = request.HarvestDate;
                entity.Address = request.Address;
                entity.IssuerUnitCode = request.IssuerUnitCode;

                // Đánh dấu modified để EF chắc chắn ghi
                var e = _context.Entry(entity);
                e.Property(x => x.CountryId).IsModified = true;
                e.Property(x => x.Code).IsModified = true;
                e.Property(x => x.Name).IsModified = true;
                e.Property(x => x.Note).IsModified = true;
                e.Property(x => x.Latitude).IsModified = true;
                e.Property(x => x.Longitude).IsModified = true;
                e.Property(x => x.Manager).IsModified = true;
                e.Property(x => x.Quantity).IsModified = true;
                e.Property(x => x.TypeId).IsModified = true;
                e.Property(x => x.Area).IsModified = true;
                e.Property(x => x.StartDate).IsModified = true;
                e.Property(x => x.HarvestDate).IsModified = true;
                e.Property(x => x.Address).IsModified = true;
                e.Property(x => x.IssuerUnitCode).IsModified = true;
                // nếu có UpdatedAt/UserUpdated trong BaseEntity:
                if (e.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    e.Property("UpdatedAt").IsModified = true;
                }

                await _context.SaveChangesAsync();
                // Trả về rỗng (idempotent: nếu không đổi cũng coi là OK)
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        public string Delete(int id)
        {
            var entity = _context.PlantingRegions.Find(id);
            if (entity != null)
            {
                _context.PlantingRegions.Remove(entity);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
