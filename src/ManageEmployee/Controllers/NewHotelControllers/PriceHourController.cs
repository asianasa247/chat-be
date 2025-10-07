using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers.NewHotelControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceHourController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public PriceHourController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PriceHourDTO model)
        {
            PriceHour priceHour = new PriceHour();
            priceHour.Code = model.Code;
            priceHour.Name = model.Name;
            priceHour.Price = model.Price;
            _dbContext.Add(priceHour);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(priceHour);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var pricehour = await _dbContext.PriceHours.FindAsync(id);
            if (pricehour != null)
            {
                _dbContext.Remove(pricehour);
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PriceHourDTO model)
        {
            var pricehour = await _dbContext.PriceHours.FindAsync(id);
            if (pricehour == null)
            {
                return NotFound();
            }
            pricehour.Code = model.Code;
            pricehour.Name = model.Name;
            pricehour.Price = model.Price;
            _dbContext.Update(pricehour);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(pricehour);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pricehour = await  (from ph in _dbContext.PriceHours
                                    select new GetPriceHour
                                    {
                                        Id = ph.Id,
                                        Name = ph.Name,
                                        Code = ph.Code,
                                        Price = ph.Price,
                                    }).ToListAsync();
            return Ok(pricehour);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pricehour = await (from ph in _dbContext.PriceHours
                                where ph.Id == id
                                select new GetPriceHour
                                {
                                    Id = ph.Id,
                                    Name = ph.Name,
                                    Code = ph.Code,
                                    Price = ph.Price,
                                }).FirstOrDefaultAsync();
            return Ok(pricehour);
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            // Đảm bảo Page và PageSize hợp lệ
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return BadRequest();
            }

            // Truy vấn dữ liệu từ bảng PriceHours
            var query = from ph in _dbContext.PriceHours
                        select new GetPriceHour
                        {
                            Id = ph.Id,
                            Name = ph.Name,
                            Code = ph.Code,
                            Price = ph.Price,
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
