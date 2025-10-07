using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.HistorySpinModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Entities.GoodsEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ManageEmployee.Controllers.NewHotelControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        public RoomController(ApplicationDbContext dbcontext) 
        {
            _dbcontext = dbcontext; 
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RoomDTO model)
        {
            Room room = new Room();
            room.Code = model.Code; 
            room.Name = model.Name;
            room.AreaId = model.AreaId;
            room.FloorId = model.FloorId;
            room.GoodId = model.GoodId;
            room.PriceDayId = model.PriceDayId;
            room.PriceHourId = model.PriceHourId;
            room.StatusHotelId = model.StatusHotelId;
            _dbcontext.Add(room);
            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(room);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var room = await _dbcontext.Rooms.FindAsync(id);
            if(room == null)
            {
                return StatusCode(500);
            }
            _dbcontext.Remove(room);
            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RoomDTO model)
        {
            var room = await _dbcontext.Rooms.FindAsync(id);
            if(room == null)
            {
                return NotFound();
            }
            room.Code = model.Code;
            room.Name = model.Name;
            room.AreaId = model.AreaId;
            room.FloorId = model.FloorId;
            room.GoodId = model.GoodId;
            room.PriceDayId = model.PriceDayId;
            room.PriceHourId = model.PriceHourId;
            room.StatusHotelId = model.StatusHotelId;
            _dbcontext.Update(room);
            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(room);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetRoom>>> GetAll()
        {
            var rooms = await _dbcontext.Rooms.ToListAsync();
            List<GetRoom> getRooms = new List<GetRoom>();
            foreach (var room in rooms)
            {
                GetRoom model = new GetRoom();
                model.RoomId = room.RoomId;
                model.Name = room.Name;
                model.Code = room.Code;
                Area area = await _dbcontext.Areas.FindAsync(room.AreaId);
                model.AreaId = new AreaInfo()
                {
                    Id = room.AreaId,
                    Code = area == null ? "No code" : area.Code,
                    Name = area == null ? "No name" : area.Name
                };

                Floor floor = await _dbcontext.Floors.FindAsync(room.FloorId);
                model.FloorId = new FloorInfo()
                {
                    Id = room.FloorId,
                    Code = floor == null ? "No code" : floor.Code,
                    Name = floor == null ? "No name" : floor.Name
                };

                StatusHotel statusHotel = await _dbcontext.StatusHotels.FindAsync(room.StatusHotelId);
                model.StatusHotelId = new StatusHotelInfo()
                {
                    Id = room.StatusHotelId,
                    Code = statusHotel == null ? "No code" : statusHotel.Code,
                    Name = statusHotel == null ? "No name" : statusHotel.Name
                };

                PriceDay priceDay = await _dbcontext.PriceDay.FindAsync(room.PriceDayId);
                model.PriceDayId = new PriceDayInfo()
                {
                    Id = room.PriceDayId,
                    Code = priceDay == null ? "No code" : priceDay.Code,
                    Name = priceDay == null ? "No name" : priceDay.Name
                };

                PriceHour priceHour = await _dbcontext.PriceHours.FindAsync(room.PriceHourId);
                model.PriceHourId = new PriceHourInfo()
                {
                    Id = room.PriceHourId,
                    Code = priceHour == null ? "No code" : priceHour.Code,
                    Name = priceHour == null ? "No name" : priceHour.Name
                };

                Goods good = await _dbcontext.Goods.FindAsync(room.GoodId);
                model.GoodId = new GoodInfomation()
                {
                    Id = room.GoodId,
                    Account = good == null ? "No account" : good.Account,
                    AccountName = good == null ? "No accountname" : good.AccountName,
                    Warehouse = good == null ? "No warehouse" : good.Warehouse,
                    WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                    Detail1 = good == null ? "No detail1" : good.Detail1,
                    DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                    Detail2 = good == null ? "No detail2" : good.Detail2,
                    DetailName2 = good == null ? "No detailName2" : good.DetailName2,
                };
                getRooms.Add(model);
            }
            return getRooms;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetRoom>> GetById(int id)
        {
            // Lấy phòng theo RoomId
            var room = await _dbcontext.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            GetRoom model = new GetRoom();
            model.RoomId = room.RoomId;
            model.Name = room.Name;
            model.Code = room.Code;

            // Lấy thông tin của các liên kết (Area, Floor, StatusHotel, v.v.)
            Area area = await _dbcontext.Areas.FindAsync(room.AreaId);
            model.AreaId = new AreaInfo()
            {
                Id = room.AreaId,
                Code = area == null ? "No code" : area.Code,
                Name = area == null ? "No name" : area.Name
            };

            Floor floor = await _dbcontext.Floors.FindAsync(room.FloorId);
            model.FloorId = new FloorInfo()
            {
                Id = room.FloorId,
                Code = floor == null ? "No code" : floor.Code,
                Name = floor == null ? "No name" : floor.Name
            };

            StatusHotel statusHotel = await _dbcontext.StatusHotels.FindAsync(room.StatusHotelId);
            model.StatusHotelId = new StatusHotelInfo()
            {
                Id = room.StatusHotelId,
                Code = statusHotel == null ? "No code" : statusHotel.Code,
                Name = statusHotel == null ? "No name" : statusHotel.Name
            };

            PriceDay priceDay = await _dbcontext.PriceDay.FindAsync(room.PriceDayId);
            model.PriceDayId = new PriceDayInfo()
            {
                Id = room.PriceDayId,
                Code = priceDay == null ? "No code" : priceDay.Code,
                Name = priceDay == null ? "No name" : priceDay.Name
            };

            PriceHour priceHour = await _dbcontext.PriceHours.FindAsync(room.PriceHourId);
            model.PriceHourId = new PriceHourInfo()
            {
                Id = room.PriceHourId,
                Code = priceHour == null ? "No code" : priceHour.Code,
                Name = priceHour == null ? "No name" : priceHour.Name
            };

            Goods good = await _dbcontext.Goods.FindAsync(room.GoodId);
            model.GoodId = new GoodInfomation()
            {
                Id = room.GoodId,
                Account = good == null ? "No account" : good.Account,
                AccountName = good == null ? "No accountname" : good.AccountName,
                Warehouse = good == null ? "No warehouse" : good.Warehouse,
                WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                Detail1 = good == null ? "No detail1" : good.Detail1,
                DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                Detail2 = good == null ? "No detail2" : good.Detail2,
                DetailName2 = good == null ? "No detailName2" : good.DetailName2,
            };

            return Ok(model);
        }


        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<GetRoom>>> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            // Kiểm tra Page và PageSize hợp lệ
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return BadRequest();
            }

            // Truy vấn danh sách phòng
            var query = from room in _dbcontext.Rooms
                        join area in _dbcontext.Areas on room.AreaId equals area.Id into areaGroup
                        from area in areaGroup.DefaultIfEmpty()
                        join floor in _dbcontext.Floors on room.FloorId equals floor.Id into floorGroup
                        from floor in floorGroup.DefaultIfEmpty()
                        join statusHotel in _dbcontext.StatusHotels on room.StatusHotelId equals statusHotel.Id into statusGroup
                        from statusHotel in statusGroup.DefaultIfEmpty()
                        join priceDay in _dbcontext.PriceDay on room.PriceDayId equals priceDay.Id into priceDayGroup
                        from priceDay in priceDayGroup.DefaultIfEmpty()
                        join priceHour in _dbcontext.PriceHours on room.PriceHourId equals priceHour.Id into priceHourGroup
                        from priceHour in priceHourGroup.DefaultIfEmpty()
                        join good in _dbcontext.Goods on room.GoodId equals good.Id into goodsGroup
                        from good in goodsGroup.DefaultIfEmpty()
                        select new GetRoom
                        {
                            RoomId = room.RoomId,
                            Name = room.Name,
                            Code = room.Code,
                            AreaId = new AreaInfo
                            {
                                Id = room.AreaId,
                                Code = area == null ? "No code" : area.Code,
                                Name = area == null ? "No name" : area.Name
                            },
                            FloorId = new FloorInfo
                            {
                                Id = room.FloorId,
                                Code = floor == null ? "No code" : floor.Code,
                                Name = floor == null ? "No name" : floor.Name
                            },
                            StatusHotelId = new StatusHotelInfo
                            {
                                Id = room.StatusHotelId,
                                Code = statusHotel == null ? "No code" : statusHotel.Code,
                                Name = statusHotel == null ? "No name" : statusHotel.Name
                            },
                            PriceDayId = new PriceDayInfo
                            {
                                Id = room.PriceDayId,
                                Code = priceDay == null ? "No code" : priceDay.Code,
                                Name = priceDay == null ? "No name" : priceDay.Name
                            },
                            PriceHourId = new PriceHourInfo
                            {
                                Id = room.PriceHourId,
                                Code = priceHour == null ? "No code" : priceHour.Code,
                                Name = priceHour == null ? "No name" : priceHour.Name
                            },
                            GoodId = new GoodInfomation
                            {
                                Id = room.GoodId,
                                Account = good == null ? "No account" : good.Account,
                                AccountName = good == null ? "No accountname" : good.AccountName,
                                Warehouse = good == null ? "No warehouse" : good.Warehouse,
                                WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                                Detail1 = good == null ? "No detail1" : good.Detail1,
                                DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                                Detail2 = good == null ? "No detail2" : good.Detail2,
                                DetailName2 = good == null ? "No detailName2" : good.DetailName2,
                            }
                        };

            // Áp dụng tìm kiếm
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(x => x.Name.Contains(pagingRequest.SearchText) || x.Code.Contains(pagingRequest.SearchText));
            }

            // Áp dụng sắp xếp nếu có
            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "code" => query.OrderBy(x => x.Code),
                    _ => query.OrderBy(x => x.RoomId) // Sắp xếp mặc định theo RoomId
                };
            }

            // Tính toán tổng số bản ghi sau tìm kiếm
            var totalItems = await query.CountAsync();

            // Phân trang
            var rooms = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize)
                .Take(pagingRequest.PageSize)
                .ToListAsync();

            // Kết quả phân trang
            var result = new
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pagingRequest.PageSize),
                CurrentPage = pagingRequest.Page,
                PageSize = pagingRequest.PageSize,
                Data = rooms
            };

            return Ok(result);
        }



    }
}
