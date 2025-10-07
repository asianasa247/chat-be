using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SettingsSpinModels;
using ManageEmployee.Entities.LotteryEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers.SettingsSpinControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsSpinController:ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        public SettingsSpinController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpPut]
        [Route("{Id:int}")]
        public async Task<IActionResult> Update([FromRoute]int Id, [FromBody] SettingsSpinDTO model)
        {
            var settingsSpin = await dbContext.SettingsSpins.FindAsync(Id);
            if (settingsSpin == null)
            {
                return NotFound();
            }
            settingsSpin.Code = model.Code;
            settingsSpin.Name = model.Name;
            settingsSpin.IdCustomerClassification = model.IdCustomerClassification;
            settingsSpin.TimeStart = model.TimeStart;
            settingsSpin.TimeEnd = model.TimeEnd;
            settingsSpin.TimeStartSpin = model.TimeStartSpin;
            settingsSpin.TimeStartPerSpin = model.TimeStartPerSpin;
            settingsSpin.TimeStopPerSpin = model.TimeStopPerSpin;
            settingsSpin.AwarDay = model.AwarDay;
            settingsSpin.Note = model.Note;

            dbContext.Update(settingsSpin);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception) 
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpDelete]
        [Route("{Id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
            var settingsSpin = await dbContext.SettingsSpins.FindAsync(Id);
            if (settingsSpin != null)
            {
                dbContext.Remove(settingsSpin);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return StatusCode(500,e.Message);
                }
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SettingsSpinDTO model)
        {
            SettingsSpin settingsSpin = model.ConvertToSettingsSpin();
            await dbContext.AddAsync(settingsSpin);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message);
            }
            return Ok();
        }

        [HttpGet("SettingSpinPage")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            if (pagingRequest.Page < 1 || pagingRequest.PageSize < 1)
                return BadRequest();

            // Xây dựng truy vấn cơ bản
            var query = from s in dbContext.SettingsSpins
                        join c in dbContext.CustomerClassifications
                        on s.IdCustomerClassification equals c.Id into queryJoin
                        from j in queryJoin.DefaultIfEmpty()
                        select new GetSettingsSpinModel
                        {
                            SettingId = s.SettingId,
                            Code = s.Code,
                            Name = s.Name,
                            IdCustomerClassification = s.IdCustomerClassification,
                            NameCustomerClassification = j == null ? "no name" : j.Name,
                            TimeStart = s.TimeStart,
                            TimeEnd = s.TimeEnd,
                            TimeStartSpin = s.TimeStartSpin,
                            TimeStartPerSpin = s.TimeStartPerSpin,
                            TimeStopPerSpin = s.TimeStopPerSpin,
                            AwarDay = s.AwarDay,
                            Note = s.Note,
                        };

            // Áp dụng SearchText nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(s => s.Name.Contains(pagingRequest.SearchText) || s.Code.Contains(pagingRequest.SearchText));
            }

            // Áp dụng sắp xếp nếu có
            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "name" => query.OrderBy(s => s.Name),
                    "code" => query.OrderBy(s => s.Code),
                    "timeStart" => query.OrderBy(s => s.TimeStart),
                    _ => query.OrderBy(s => s.SettingId)  // Nếu không có trường hợp hợp lệ, mặc định là theo SettingId
                };
            }

            // Tính tổng số item
            var totalItems = await query.CountAsync();

            // Lấy dữ liệu theo phân trang
            var settingsSpins = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize)  // Tính toán số lượng phần tử bỏ qua
                .Take(pagingRequest.PageSize)  // Lấy số lượng phần tử
                .ToListAsync();

            // Trả về kết quả phân trang
            var result = new
            {
                PageNumber = pagingRequest.Page,
                PageSize = pagingRequest.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pagingRequest.PageSize),
                Data = settingsSpins
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            // Kiểm tra nếu Id không hợp lệ
            if (Id <= 0)
            {
                return BadRequest();
            }

            // Lấy dữ liệu SettingsSpin dựa trên Id
            var settingsSpin = await dbContext.SettingsSpins
                                               .Where(s => s.SettingId == Id)
                                               .Select(s => new GetSettingsSpinModel
                                               {
                                                   SettingId = s.SettingId,
                                                   Code = s.Code,
                                                   Name = s.Name,
                                                   IdCustomerClassification = s.IdCustomerClassification,
                                                   NameCustomerClassification = dbContext.CustomerClassifications
                                                                                         .Where(c => c.Id == s.IdCustomerClassification)
                                                                                         .Select(c => c.Name)
                                                                                         .FirstOrDefault() ?? "No name",
                                                   TimeStart = s.TimeStart,
                                                   TimeEnd = s.TimeEnd,
                                                   TimeStartSpin = s.TimeStartSpin,
                                                   TimeStartPerSpin = s.TimeStartPerSpin,
                                                   TimeStopPerSpin = s.TimeStopPerSpin,
                                                   AwarDay = s.AwarDay,
                                                   Note = s.Note,
                                               })
                                               .FirstOrDefaultAsync();

            // Nếu không tìm thấy dữ liệu, trả về NotFound
            if (settingsSpin == null)
            {
                return NotFound();
            }

            // Trả về dữ liệu tìm thấy
            return Ok(settingsSpin);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var settingsSpins = await (from s in dbContext.SettingsSpins
                                       //join c in dbContext.CustomerClassifications
                                       //on s.IdCustomerClassification equals c.Id
                                       //into query
                                       //from j in query.DefaultIfEmpty()
                                       select new GetSettingsSpinModel()
                                       {
                                           SettingId = s.SettingId,
                                           Code = s.Code,
                                           Name = s.Name,
                                           IdCustomerClassification = s.IdCustomerClassification,
                                           //NameCustomerClassification = j == null ? "no name" : j.Name,
                                           TimeStart = s.TimeStart,
                                           TimeEnd = s.TimeEnd,
                                           TimeStartSpin = s.TimeStartSpin,
                                           TimeStartPerSpin = s.TimeStartPerSpin,
                                           TimeStopPerSpin = s.TimeStopPerSpin,
                                           AwarDay = s.AwarDay,
                                           Note = s.Note,
                                       }).ToListAsync();
            return Ok(settingsSpins);
        }
    }
}
