using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;
using ManageEmployee.Services.Interfaces.Cultivation;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Cultivation
{
    public class PlantingBedService : IPlantingBedService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlantingBedService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagingResult<PlantingBed>> GetAll(int page, int pageSize, string keyword, int? regionId = null, int? typeId = null)
        {
            if (pageSize <= 0) pageSize = 20;
            if (page < 1) page = 1;

            IQueryable<PlantingBed> query = _context.PlantingBeds.AsNoTracking();

            if (regionId.HasValue) query = query.Where(x => x.RegionId == regionId.Value);
            if (typeId.HasValue) query = query.Where(x => x.TypeId == typeId.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Code.Contains(keyword) ||
                    x.Name.Contains(keyword) ||
                    (x.Note != null && x.Note.Contains(keyword)) ||
                    x.Type.Name.Contains(keyword) ||
                    x.Type.Code.Contains(keyword));
            }

            var result = new PagingResult<PlantingBed>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query
                    .Include(x => x.Region)
                    .Include(x => x.Type)
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };

            return result;
        }

        public IEnumerable<PlantingBed> GetAll()
        {
            return _context.PlantingBeds
                .AsNoTracking()
                .Include(x => x.Region)
                .Include(x => x.Type)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public async Task<string> Create(PlantingBed request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                // Region & Type phải tồn tại
                var regionExists = await _context.PlantingRegions.AnyAsync(r => r.Id == request.RegionId);
                if (!regionExists) return "RegionNotFound";
                var typeExists = await _context.PlantingTypes.AnyAsync(t => t.Id == request.TypeId);
                if (!typeExists) return "TypeNotFound";

                // Unique (RegionId, Code)
                var dup = await _context.PlantingBeds.AnyAsync(x =>
                    x.RegionId == request.RegionId &&
                    x.Code.ToLower() == request.Code.ToLower());
                if (dup) return "Bed_CodeAlreadyExist";

                var entity = _mapper.Map<PlantingBed>(request);
                _context.PlantingBeds.Add(entity);
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

        public PlantingBed GetById(int id) => _context.PlantingBeds.Find(id)!;

        public async Task<string> Update(PlantingBed request)
        {
            try
            {
                request.Region = null;
                request.Type = null;

                var entity = await _context.PlantingBeds
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null) return "Bed_NotFound";

                // Region & Type phải tồn tại
                var regionExists = await _context.PlantingRegions.AnyAsync(r => r.Id == request.RegionId);
                if (!regionExists) return "RegionNotFound";
                var typeExists = await _context.PlantingTypes.AnyAsync(t => t.Id == request.TypeId);
                if (!typeExists) return "TypeNotFound";

                // Unique (RegionId, Code) – trừ bản ghi hiện tại
                var dup = await _context.PlantingBeds.AnyAsync(x =>
                    x.Id != request.Id &&
                    x.RegionId == request.RegionId &&
                    x.Code.ToLower() == request.Code.ToLower());
                if (dup) return "Bed_CodeAlreadyExist";

                // Patch
                entity.RegionId = request.RegionId;
                entity.Code = request.Code;
                entity.Name = request.Name;
                entity.Note = request.Note;
                entity.Latitude = request.Latitude;
                entity.Longitude = request.Longitude;
                entity.Quantity = request.Quantity;
                entity.StartYear = request.StartYear;
                entity.TypeId = request.TypeId;
                entity.HarvestDate = request.HarvestDate;

                var e = _context.Entry(entity);
                e.Property(x => x.RegionId).IsModified = true;
                e.Property(x => x.Code).IsModified = true;
                e.Property(x => x.Name).IsModified = true;
                e.Property(x => x.Note).IsModified = true;
                e.Property(x => x.Latitude).IsModified = true;
                e.Property(x => x.Longitude).IsModified = true;
                e.Property(x => x.Quantity).IsModified = true;
                e.Property(x => x.StartYear).IsModified = true;
                e.Property(x => x.TypeId).IsModified = true;
                e.Property(x => x.HarvestDate).IsModified = true;
                if (e.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    e.Property("UpdatedAt").IsModified = true;
                }

                await _context.SaveChangesAsync();
                // Trả về rỗng để controller trả 200 OK (idempotent)
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        public string Delete(int id)
        {
            var entity = _context.PlantingBeds.Find(id);
            if (entity != null)
            {
                _context.PlantingBeds.Remove(entity);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
    