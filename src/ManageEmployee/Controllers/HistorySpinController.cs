using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.HistorySpinModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LotteryEntities;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistorySpinController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        private readonly IFileService _fileService;
        private readonly ISpinPrizeRandomService _spinPrizeRandomService;

        public HistorySpinController(ApplicationDbContext dbContext, IFileService fileService, ISpinPrizeRandomService spinPrizeRandomService)
        {
            this.dbContext = dbContext;
            _fileService = fileService;
            _spinPrizeRandomService = spinPrizeRandomService;
        }


        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute]int Id, [FromBody]HistorySpinDTO model)
        {
            var historySpin = await dbContext.HistorySpinDetails.FindAsync(Id);
            if (historySpin == null)
            {
                return NotFound();
            }
            historySpin.SettingsSpinId = model.IdSettingsSpin;
            historySpin.PrizeId = model.PrizeId;
            historySpin.CustomerId = model.CustomerId;
            historySpin.GoodId = model.GoodId;
            historySpin.WinTime =  model.WinTime ?? DateTime.Now;
            historySpin.ReceivedDay = model.ReceivedDay ?? DateTime.Now;
            var files = model.UploadedFiles ?? new List<FileDetailModel>();
            if (model.File != null && model.File.Any())
            {
                foreach (var file in model.File)
                {
                    var fileUrl = _fileService.Upload(file, "HistorySpin", file.FileName);
                    files.Add(new FileDetailModel
                    {
                        FileName = file.FileName,
                        FileUrl = fileUrl,
                    });
                }
            }
            historySpin.Image = JsonConvert.SerializeObject(files).ToString();

            dbContext.Update(historySpin);
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] HistorySpinDTO model)
        {
            var history = new HistorySpin
            {
                CreatedAt = DateTime.Now,
                IdSettingsSpin = model.IdSettingsSpin,
                WinTime = model.WinTime ?? DateTime.Now,
                ReceivedDay = model.ReceivedDay ?? DateTime.Now,
            };
            await dbContext.AddAsync(history);
            await dbContext.SaveChangesAsync();

            HistorySpinDetail historySpin = new HistorySpinDetail();
            historySpin.HistorySpinId = history.HistoryId;
            historySpin.SettingsSpinId = model.IdSettingsSpin;
            historySpin.PrizeId = model.PrizeId;
            historySpin.CustomerId = model.CustomerId;
            historySpin.GoodId = model.GoodId;
            historySpin.WinTime = model.WinTime ?? DateTime.Now;
            historySpin.ReceivedDay = model.ReceivedDay ?? DateTime.Now;
            var files = model.UploadedFiles ?? new List<FileDetailModel>();
            if (model.File != null && model.File.Any())
            {
                foreach (var file in model.File)
                {
                    var fileUrl = _fileService.Upload(file, "HistorySpin", file.FileName);
                    files.Add(new FileDetailModel
                    {
                        FileName = file.FileName,
                        FileUrl = fileUrl,
                    });
                }
            }
            historySpin.Image = JsonConvert.SerializeObject(files).ToString();

            await dbContext.AddAsync(historySpin);
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetHistorySpinModel>>> Get()
        {

            var histories = await dbContext.HistorySpinDetails.ToListAsync();
            List<GetHistorySpinModel> result = new List<GetHistorySpinModel>(); 
            foreach (var history in histories)
            {
                GetHistorySpinModel model = new GetHistorySpinModel();
                model.HistoryId = history.Id;
                SettingsSpin settingSpin = await dbContext.SettingsSpins.FindAsync(history.SettingsSpinId);
                model.SettingsSpinInfo = new SettingSpinInfo()
                {
                    Id = history.SettingsSpinId,
                    Code = settingSpin==null?"No code":settingSpin.Code,
                    Name = settingSpin == null ? "No Name" : settingSpin.Name
                };
                var prize = await dbContext.Prizes.FindAsync(history.PrizeId);
                model.Prize = new PrizeInfo() 
                { 
                    Id = history.PrizeId,
                    Code = prize == null? "No Code":prize.Code,
                    Name = prize == null ? "No Name" : prize.Name
                };
                var customer = await dbContext.Customers.FindAsync(history.CustomerId);
                model.Customer = new CustomerInfo()
                {
                    Id = history.CustomerId,
                    Code = customer == null ? "No Code" : customer.Code,
                    Name = customer == null ? "No Name" : customer.Name
                };
                var good = await dbContext.Goods.FindAsync(history.GoodId);
                model.Good = new GoodInfo() 
                {
                    Id = history.GoodId,
                    Account = good ==null?"No account":good.Account,
                    AccountName = good == null ? "No accountname" : good.AccountName,
                    Warehouse = good == null ? "No warehouse" : good.Warehouse,
                    WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                    Detail1 = good == null ? "No detail1" : good.Detail1,
                    DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                    Detail2 = good == null ? "No detail2" : good.Detail2,
                    DetailName2 = good == null ? "No detailName2" : good.DetailName2,
                };
                model.WinTime = history.WinTime;
                model.ReceivedDay = history.ReceivedDay;
                model.Image = history.Image;
                result.Add(model);
            }
            return result;
        }



        [HttpGet("HistorySpinPage")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            // Kiểm tra các tham số phân trang
            if (pagingRequest.Page < 1 || pagingRequest.PageSize < 1)
                return BadRequest();

            // Lấy tổng số bản ghi
            var query = dbContext.HistorySpinDetails.AsQueryable();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(pagingRequest.SearchText))
            {
                query = query.Where(h => h.HistorySpinId.ToString().Contains(pagingRequest.SearchText) ||
                                         h.Image.Contains(pagingRequest.SearchText)); // Tìm kiếm theo HistoryId hoặc Image
            }

            // Lấy tổng số bản ghi sau khi áp dụng tìm kiếm
            var totalItems = await query.CountAsync();

            // Áp dụng sắp xếp nếu có
            if (pagingRequest.isSort && !string.IsNullOrEmpty(pagingRequest.SortField))
            {
                query = pagingRequest.SortField.ToLower() switch
                {
                    "historyid" => query.OrderBy(h => h.HistorySpinId),
                    "image" => query.OrderBy(h => h.Image),
                    _ => query.OrderBy(h => h.HistorySpinId) // Sắp xếp theo HistoryId mặc định
                };
            }

            // Phân trang dữ liệu
            var histories = await query
                .Skip((pagingRequest.Page - 1) * pagingRequest.PageSize) // Số mục cần bỏ qua
                .Take(pagingRequest.PageSize) // Số mục cần lấy
                .ToListAsync();

            // Lấy chi tiết liên quan cho mỗi lịch sử
            List<GetHistorySpinModel> result = new List<GetHistorySpinModel>();
            foreach (var history in histories)
            {
                GetHistorySpinModel model = new GetHistorySpinModel
                {
                    HistoryId = history.Id,
                    WinTime = history.WinTime,
                    ReceivedDay = history.ReceivedDay,
                    Image = history.Image
                };

                // Lấy thông tin liên quan
                var settingSpin = await dbContext.SettingsSpins.FindAsync(history.SettingsSpinId);
                model.SettingsSpinInfo = new SettingSpinInfo()
                {
                    Id = history.SettingsSpinId,
                    Code = settingSpin == null ? "No code" : settingSpin.Code,
                    Name = settingSpin == null ? "No Name" : settingSpin.Name
                };

                var prize = await dbContext.Prizes.FindAsync(history.PrizeId);
                model.Prize = new PrizeInfo()
                {
                    Id = history.PrizeId,
                    Code = prize == null ? "No Code" : prize.Code,
                    Name = prize == null ? "No Name" : prize.Name
                };

                var customer = await dbContext.Customers.FindAsync(history.CustomerId);
                model.Customer = new CustomerInfo()
                {
                    Id = history.CustomerId,
                    Code = customer == null ? "No Code" : customer.Code,
                    Name = customer == null ? "No Name" : customer.Name
                };

                var good = await dbContext.Goods.FindAsync(history.GoodId);
                model.Good = new GoodInfo()
                {
                    Id = history.GoodId,
                    Account = good == null ? "No account" : good.Account,
                    AccountName = good == null ? "No accountname" : good.AccountName,
                    Warehouse = good == null ? "No warehouse" : good.Warehouse,
                    WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                    Detail1 = good == null ? "No detail1" : good.Detail1,
                    DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                    Detail2 = good == null ? "No detail2" : good.Detail2,
                    DetailName2 = good == null ? "No detailName2" : good.DetailName2
                };

                result.Add(model);
            }

            // Tạo đối tượng phản hồi phân trang
            var response = new
            {
                PageNumber = pagingRequest.Page,
                PageSize = pagingRequest.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pagingRequest.PageSize),
                Data = result
            };

            return Ok(response);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            var history = await dbContext.HistorySpinDetails.FindAsync(Id);
            if (history == null)
            {
                return NotFound();
            }

            GetHistorySpinModel model = new GetHistorySpinModel();
            model.HistoryId = history.Id;

            // Lấy thông tin từ bảng SettingsSpins
            var settingSpin = await dbContext.SettingsSpins.FindAsync(history.SettingsSpinId);
            model.SettingsSpinInfo = new SettingSpinInfo()
            {
                Id = history.SettingsSpinId,
                Code = settingSpin == null ? "No code" : settingSpin.Code,
                Name = settingSpin == null ? "No Name" : settingSpin.Name
            };

            // Lấy thông tin từ bảng Prizes
            var prize = await dbContext.Prizes.FindAsync(history.PrizeId);
            model.Prize = new PrizeInfo()
            {
                Id = history.PrizeId,
                Code = prize == null ? "No Code" : prize.Code,
                Name = prize == null ? "No Name" : prize.Name
            };

            // Lấy thông tin từ bảng Customers
            var customer = await dbContext.Customers.FindAsync(history.CustomerId);
            model.Customer = new CustomerInfo()
            {
                Id = history.CustomerId,
                Code = customer == null ? "No Code" : customer.Code,
                Name = customer == null ? "No Name" : customer.Name
            };

            // Lấy thông tin từ bảng Goods
            var good = await dbContext.Goods.FindAsync(history.GoodId);
            model.Good = new GoodInfo()
            {
                Id = history.GoodId,
                Account = good == null ? "No account" : good.Account,
                AccountName = good == null ? "No account name" : good.AccountName,
                Warehouse = good == null ? "No warehouse" : good.Warehouse,
                WarehouseName = good == null ? "No warehouse name" : good.WarehouseName,
                Detail1 = good == null ? "No detail1" : good.Detail1,
                DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                Detail2 = good == null ? "No detail2" : good.Detail2,
                DetailName2 = good == null ? "No detailName2" : good.DetailName2,
            };

            model.WinTime = history.WinTime;
            model.ReceivedDay = history.ReceivedDay;
            model.Image = history.Image;

            return Ok(model);
        }

        [HttpPost("random")]
        public async Task<IActionResult> RandomPrize()
        {
            await _spinPrizeRandomService.RandomAsync();
            return Ok();
        }

    }
}
