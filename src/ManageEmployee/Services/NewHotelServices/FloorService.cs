using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Services.Interfaces.NewHotels;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.NewHotelServices
{
    public class FloorService : IFloorService
    {
        private readonly ApplicationDbContext _dbContext;

        public FloorService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Floor> Add(FloorDTO model)
        {
            var floor = new Floor
            {
                Code = model.Code,
                Name = model.Name,
                AreaId = model.AreaId
            };

            _dbContext.Floors.Add(floor);
            await _dbContext.SaveChangesAsync();
            return floor;
        }

        public async Task<Floor> Update(int id, FloorDTO model)
        {
            var floor = await _dbContext.Floors.FindAsync(id);
            if (floor == null) return null;

            floor.Code = model.Code;
            floor.Name = model.Name;
            floor.AreaId = model.AreaId;
            _dbContext.Floors.Update(floor);
            await _dbContext.SaveChangesAsync();
            return floor;
        }

        public async Task<bool> Delete(int id)
        {
            var floor = await _dbContext.Floors.FindAsync(id);
            if (floor == null) return false;

            _dbContext.Floors.Remove(floor);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<GetFloorModel>> GetAll()
        {
            return await (from floor in _dbContext.Floors
                          join area in _dbContext.Areas on floor.AreaId equals area.Id into query
                          from nameArea in query.DefaultIfEmpty()
                          select new GetFloorModel
                          {
                              Id = floor.Id,
                              Name = floor.Name,
                              Code = floor.Code,
                              AreaId = floor.AreaId,
                              AreaName = nameArea == null ? "no name" : nameArea.Name,
                          }).ToListAsync();
        }

        public async Task<GetFloorModel> GetById(int id)
        {
            return await (from floor in _dbContext.Floors
                          join area in _dbContext.Areas on floor.AreaId equals area.Id into query
                          from nameArea in query.DefaultIfEmpty()
                          where floor.Id == id
                          select new GetFloorModel
                          {
                              Id = floor.Id,
                              Name = floor.Name,
                              Code = floor.Code,
                              AreaId = floor.AreaId,
                              AreaName = nameArea == null ? "no name" : nameArea.Name,
                          }).FirstOrDefaultAsync();
        }

        public async Task<object> GetPaged(PagingRequestModel pagingRequest)
        {
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                throw new ArgumentException("Page and PageSize must be greater than zero.");
            }

            var query = from floor in _dbContext.Floors
                        join area in _dbContext.Areas on floor.AreaId equals area.Id into areaQuery
                        from nameArea in areaQuery.DefaultIfEmpty()
                        select new GetFloorModel
                        {
                            Id = floor.Id,
                            Name = floor.Name,
                            Code = floor.Code,
                            AreaId = floor.AreaId,
                            AreaName = nameArea == null ? "no name" : nameArea.Name,
                        };

            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(x => x.Name.Contains(pagingRequest.SearchText) ||
                                         x.Code.Contains(pagingRequest.SearchText) ||
                                         x.AreaName.Contains(pagingRequest.SearchText));
            }

            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "code" => query.OrderBy(x => x.Code),
                    "areaname" => query.OrderBy(x => x.AreaName),
                    _ => query.OrderBy(x => x.Id)
                };
            }

            var totalItems = await query.CountAsync();
            var floors = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize)
                .Take(pagingRequest.PageSize)
                .ToListAsync();

            return new
            {
                TotalItems = totalItems,
                PageNumber = pagingRequest.Page,
                PageSize = pagingRequest.PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pagingRequest.PageSize),
                Data = floors
            };
        }
    }

}
