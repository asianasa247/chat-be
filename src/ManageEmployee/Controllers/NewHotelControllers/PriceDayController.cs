using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ManageEmployee.Controllers.NewHotelControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceDayController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public PriceDayController(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PriceDayDTO model)
        {
            PriceDay priceDay = new PriceDay();
            priceDay.Code = model.Code;
            priceDay.Name = model.Name; 
            priceDay.Price = model.Price;
            priceDay.FromHour = model.FromHour ?? DateTime.Now;
            priceDay.ToHour = model.ToHour ?? DateTime.Now;
            _dbContext.Add(priceDay);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(priceDay);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var priceday = await (from pd in _dbContext.PriceDay
                                  select new GetPriceDay
                                  {
                                      Id = pd.Id,
                                      Name = pd.Name,
                                      Code = pd.Code,
                                      Price = pd.Price,
                                      FromHour = pd.FromHour,
                                      ToHour = pd.ToHour,
                                  }
                               ).ToListAsync();
            return Ok(priceday);
        }
        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PriceDayDTO model)
        {
            var priceday = await _dbContext.PriceDay.FindAsync(Id);
            if (priceday == null)
            {
                return NotFound();
            }
            priceday.Code = model.Code;
            priceday.Name = model.Name;
            priceday.Price = model.Price;
            priceday.FromHour = model.FromHour ?? DateTime.Now;
            priceday.ToHour = model.ToHour ?? DateTime.Now;
            _dbContext.Update(priceday);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok(priceday);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var priceday = await (from pd in _dbContext.PriceDay
                                where pd.Id == id
                                select new GetPriceDay
                                {
                                    Id = pd.Id,
                                    Name = pd.Name,
                                    Code = pd.Code,
                                    Price = pd.Price,
                                    FromHour = pd.FromHour,
                                    ToHour = pd.ToHour,
                                }
                                ).FirstOrDefaultAsync();
                               
            return Ok(priceday);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var priceday = await _dbContext.PriceDay.FindAsync(id);
            if(priceday != null)
            {
                _dbContext.Remove(priceday);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            // Đảm bảo Page và PageSize hợp lệ
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return BadRequest();
            }

            // Truy vấn dữ liệu từ bảng PriceDay
            var query = from pd in _dbContext.PriceDay
                        select new GetPriceDay
                        {
                            Id = pd.Id,
                            Name = pd.Name,
                            Code = pd.Code,
                            Price = pd.Price,
                            FromHour = pd.FromHour,
                            ToHour = pd.ToHour,
                        };

            // Áp dụng tìm kiếm
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(x => x.Name.Contains(pagingRequest.SearchText) || x.Code.Contains(pagingRequest.SearchText));
            }

            // Áp dụng sắp xếp nếu có yêu cầu
            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "code" => query.OrderBy(x => x.Code),
                    "price" => query.OrderBy(x => x.Price),
                    _ => query.OrderBy(x => x.Id) // Sắp xếp mặc định theo Id
                };
            }

            // Tính toán tổng số bản ghi sau tìm kiếm
            var totalCount = await query.CountAsync();

            // Lấy dữ liệu phân trang
            var status = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize)
                .Take(pagingRequest.PageSize)
                .ToListAsync();

            // Tạo đối tượng kết quả phân trang
            var pagedResponse = new
            {
                TotalCount = totalCount,  // Tổng số bản ghi
                PageNumber = pagingRequest.Page,  // Trang hiện tại
                PageSize = pagingRequest.PageSize,  // Số lượng bản ghi mỗi trang
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagingRequest.PageSize),  // Tổng số trang
                Data = status  // Dữ liệu của trang hiện tại
            };

            // Trả về kết quả phân trang
            return Ok(pagedResponse);
        }

    }
}
