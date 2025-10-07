using Google;
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

    public class StatusHotelController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public StatusHotelController(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] StatusHotelDTO model)
        {
            StatusHotel statusHotel = new StatusHotel();
            statusHotel.Code = model.Code;
            statusHotel.Name = model.Name;
            _dbContext.Add(statusHotel);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok(statusHotel);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var status = await _dbContext.StatusHotels.FindAsync(id);
            if(status != null)
            {
                _dbContext.Remove(status);
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
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StatusHotelDTO model)
        {
            var status = await _dbContext.StatusHotels.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            status.Code = model.Code;
            status.Name = model.Name;
            _dbContext.Update(status);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok(status);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var status = await (from st in _dbContext.StatusHotels
                               select new GetStatusModel
                               {
                                   Id = st.Id,
                                   Name = st.Name,
                                   Code = st.Code,
                               }).ToListAsync();
            return Ok(status);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult>GetById(int id)
        {
            var status = await (from st in _dbContext.StatusHotels
                                where st.Id == id
                                select new GetStatusModel
                                {
                                    Id = st.Id,
                                    Name = st.Name,
                                    Code = st.Code,
                                }).FirstOrDefaultAsync();
            return Ok(status);
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            // Kiểm tra tham số đầu vào
            if (pagingRequest.Page <= 0 || pagingRequest.PageSize <= 0)
            {
                return BadRequest();
            }

            // Truy vấn dữ liệu từ bảng StatusHotels
            var query = from st in _dbContext.StatusHotels
                        select new GetStatusModel
                        {
                            Id = st.Id,
                            Name = st.Name,
                            Code = st.Code,
                        };

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(st => st.Name.Contains(pagingRequest.SearchText) || st.Code.Contains(pagingRequest.SearchText));
            }

            // Áp dụng sắp xếp nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => pagingRequest.isSort ? query.OrderBy(st => st.Name) : query.OrderByDescending(st => st.Name),
                    "code" => pagingRequest.isSort ? query.OrderBy(st => st.Code) : query.OrderByDescending(st => st.Code),
                    _ => query.OrderBy(st => st.Id) // Sắp xếp mặc định theo Id
                };
            }
            else
            {
                query = query.OrderBy(st => st.Id); // Sắp xếp mặc định theo Id nếu không có trường sắp xếp
            }

            // Tính tổng số bản ghi sau tìm kiếm
            var totalCount = await query.CountAsync();

            // Lấy dữ liệu phân trang
            var status = await query
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
                Data = status  // Dữ liệu của trang hiện tại
            };

            // Trả về kết quả phân trang
            return Ok(pagedResponse);
        }



    }
}
