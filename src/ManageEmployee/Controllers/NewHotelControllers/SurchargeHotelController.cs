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
    
    public class SurchargeHotelController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public SurchargeHotelController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SurchargeHotelDTO model)
        {
            SurchargeHotel surcharge = new SurchargeHotel();
            surcharge.Code = model.Code;
            surcharge.Name = model.Name;
            surcharge.Price = model.Price;
            surcharge.FromHour = model.FromHour ?? DateTime.Now;
            surcharge.ToHour = model.ToHour ?? DateTime.Now;
            _dbContext.Add(surcharge);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(surcharge);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var surcharge = await (from pd in _dbContext.SurchargeHotels
                                  select new GetSurchargeHotel
                                  {
                                      Id = pd.Id,
                                      Name = pd.Name,
                                      Code = pd.Code,
                                      Price = pd.Price,
                                      FromHour = pd.FromHour,
                                      ToHour = pd.ToHour,
                                  }
                               ).ToListAsync();
            return Ok(surcharge);
        }
        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] SurchargeHotelDTO model)
        {
            var surcharge = await _dbContext.SurchargeHotels.FindAsync(Id);
            if (surcharge == null)
            {
                return NotFound();
            }
            surcharge.Code = model.Code;
            surcharge.Name = model.Name;
            surcharge.Price = model.Price;
            surcharge.FromHour = model.FromHour ?? DateTime.Now;
            surcharge.ToHour = model.ToHour ?? DateTime.Now;
            _dbContext.Update(surcharge);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok(surcharge);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var surcharge = await (from pd in _dbContext.SurchargeHotels
                                  where pd.Id == id
                                  select new GetSurchargeHotel
                                  {
                                      Id = pd.Id,
                                      Name = pd.Name,
                                      Code = pd.Code,
                                      Price = pd.Price,
                                      FromHour = pd.FromHour,
                                      ToHour = pd.ToHour,
                                  }
                                ).FirstOrDefaultAsync();

            return Ok(surcharge);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var surcharge = await _dbContext.SurchargeHotels.FindAsync(id);
            if (surcharge != null)
            {
                _dbContext.Remove(surcharge);
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
            // Kiểm tra tham số đầu vào
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return BadRequest();
            }

            // Truy vấn dữ liệu từ bảng SurchargeHotels
            var query = from pd in _dbContext.SurchargeHotels
                        select new GetSurchargeHotel
                        {
                            Id = pd.Id,
                            Name = pd.Name,
                            Code = pd.Code,
                            Price = pd.Price,
                            FromHour = pd.FromHour,
                            ToHour = pd.ToHour,
                        };

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(pd => pd.Name.Contains(pagingRequest.SearchText) || pd.Code.Contains(pagingRequest.SearchText));
            }

            // Áp dụng sắp xếp nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => pagingRequest.isSort ? query.OrderBy(pd => pd.Name) : query.OrderByDescending(pd => pd.Name),
                    "code" => pagingRequest.isSort ? query.OrderBy(pd => pd.Code) : query.OrderByDescending(pd => pd.Code),
                    "price" => pagingRequest.isSort ? query.OrderBy(pd => pd.Price) : query.OrderByDescending(pd => pd.Price),
                    _ => query.OrderBy(pd => pd.Id) // Sắp xếp mặc định theo Id
                };
            }
            else
            {
                query = query.OrderBy(pd => pd.Id); // Sắp xếp mặc định theo Id nếu không có trường sắp xếp
            }

            // Tính tổng số bản ghi sau tìm kiếm
            var totalCount = await query.CountAsync();

            // Lấy dữ liệu phân trang
            var surcharges = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize)  // Bỏ qua các bản ghi của các trang trước
                .Take(pagingRequest.PageSize)  // Lấy số lượng bản ghi theo PageSize
                .ToListAsync();

            // Tạo đối tượng chứa thông tin phân trang và dữ liệu
            var pagedResponse = new
            {
                TotalCount = totalCount,  // Tổng số bản ghi
                PageNumber = pagingRequest.Page,  // Trang hiện tại
                PageSize = pagingRequest.PageSize,  // Số lượng bản ghi mỗi trang
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagingRequest.PageSize),  // Tổng số trang
                Data = surcharges  // Dữ liệu của trang hiện tại
            };

            // Trả về kết quả phân trang
            return Ok(pagedResponse);
        }

    }
}
