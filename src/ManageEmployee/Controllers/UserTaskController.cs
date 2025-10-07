using AutoMapper;
using Common.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Claims;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Controllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserTaskController : ControllerBase
{
    private readonly IUserTaskService _userTaskService;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

    public UserTaskController(ApplicationDbContext context,
                            IUserTaskService userTaskService, IMapper mapper, IHubContext<BroadcastHub, IHubClient> hubContext)
    {
        _userTaskService = userTaskService;
        _mapper = mapper;
        _context = context;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserTaskModel param)
    {
        var currentUser = HttpContext.GetIdentityUser();

        if (param.DepartmentId == -2)
            param.ViewAll = true;

        string fullName = currentUser.FullName;

        var entityUserTask = _mapper.Map<UserTask>(param);
        var CheckLists = _mapper.Map<List<UserTaskCheckList>>(param.CheckList);

        var TaskRole = _mapper.Map<List<UserTaskRoleDetails>>(param.TaskRole);
        var FileLink = param.FileLink;

        var result = await _userTaskService.Add(entityUserTask, CheckLists, TaskRole, FileLink, currentUser.Id);

        if (result != null)
        {
            if (param.DepartmentId == -2)
            {
                var users = await _context.Users.Where(x => !x.IsDelete && x.Id != currentUser.Id).ToListAsync();
                users.ForEach(role =>
                {
                    BillTracking billTracking = new()
                    {
                        BillId = 0,
                        CustomerName = role.Id.ToString(),
                        UserCode = role.Id.ToString(),
                        UserIdReceived = param.Id,
                        Note = "<strong>" + param.Name + "</strong><br/>" + fullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                        TranType = TranTypeConst.SendToStaff,
                        Status = "Success",
                        IsRead = false,
                        DisplayOrder = 0,
                    };
                    _context.BillTrackings.Add(billTracking);
                });
            }
            else if (param.DepartmentId == -1)
            {
                if (param.TaskRole != null)
                {
                    param.TaskRole.ForEach(role =>
                    {
                        if (role.UserId != currentUser.Id)
                        {
                            BillTracking billTracking = new()
                            {
                                BillId = 0,
                                CustomerName = role.UserId.ToString(),
                                UserCode = role.UserId.ToString(),
                                UserIdReceived = param.Id,
                                Note = "<strong>" + param.Name + "</strong><br/>" + fullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                                TranType = TranTypeConst.SendToStaff,
                                Status = "Success",
                                IsRead = false,
                                DisplayOrder = 0
                            };
                            _context.BillTrackings.Add(billTracking);
                        }
                    });
                }
            }
            else if (param.DepartmentId > 0)
            {
                var listUser = await _context.Users.Where(x => x.DepartmentId == param.DepartmentId && !x.IsDelete && x.Id != currentUser.Id).ToListAsync();
                foreach (var userItem in listUser)
                {
                    BillTracking billTracking = new BillTracking()
                    {
                        BillId = 0,
                        CustomerName = userItem.Id.ToString(),
                        UserCode = userItem.Id.ToString(),
                        UserIdReceived = param.Id,
                        Note = "<strong>" + param.Name + "</strong><br/>" + fullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                        TranType = TranTypeConst.SendToStaff,
                        Status = "Success",
                        IsRead = false,
                        DisplayOrder = 0
                    };
                    _context.BillTrackings.Add(billTracking);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
               //await _hubContext.Clients.All.BroadcastMessage();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }
        return Ok(result);
    }

    [HttpPost("copy/{id}")]
    public async Task<IActionResult> Copy(int id)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity)
            {
                var result = await _userTaskService.Copy(id);
                if (result != null)
                    return Ok(result);
            }
            return BadRequest("Bạn không có quyền truy cập.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserTaskModel model)
    {
        var currentUser = HttpContext.GetIdentityUser();

        if (model.DepartmentId == -2)
            model.ViewAll = true;

        var entityUserTask = _mapper.Map<UserTask>(model);
        var CheckLists = _mapper.Map<List<UserTaskCheckList>>(model.CheckList);
        var TaskRole = _mapper.Map<List<UserTaskRoleDetails>>(model.TaskRole);
        var FileLink = _mapper.Map<List<UserTaskFileModel>>(model.FileLink);

        var result = await _userTaskService.Edit(entityUserTask, CheckLists, TaskRole, FileLink);

        if (result != null)
        {
            try
            {
                string fullName = currentUser.FullName;

                var listbillTracking = await _context.BillTrackings.Where(x => x.UserIdReceived == model.Id && !x.IsRead).ToListAsync();

                if (model.ViewAll == true)
                {
                    var users = await _context.Users.Where(x => !x.IsDelete && x.Id != currentUser.Id).ToListAsync();
                    users.ForEach(role =>
                    {
                        var billTracking = listbillTracking.Find(x => x.UserCode == role.Id.ToString());
                        if (billTracking == null)
                        {
                            billTracking = new BillTracking()
                            {
                                BillId = 0,
                                CustomerName = role.Id.ToString(),
                                UserCode = role.Id.ToString(),
                                UserIdReceived = model.Id,
                                Note = "<strong>" + model.Name + "</strong><br/>" + fullName + "<br/>"
                                            + model.Description + "<br/>"
                                            + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                                TranType = TranTypeConst.SendToStaff,
                                Status = "Success",
                                IsRead = false,
                                DisplayOrder = 0
                            };
                            _context.BillTrackings.Add(billTracking);
                        }
                    });
                }
                else
                {
                    model.TaskRole.ForEach(role =>
                    {
                        var billTracking = listbillTracking.FirstOrDefault(x => x.UserCode == role.UserId.ToString());
                        if (billTracking == null)
                        {
                            billTracking = new BillTracking()
                            {
                                BillId = 0,
                                CustomerName = role.UserId.ToString(),
                                UserCode = role.UserId.ToString(),
                                UserIdReceived = model.Id,
                                Note = "<strong>" + model.Name + "</strong><br/>" + fullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                                TranType = TranTypeConst.SendToStaff,
                                Status = "Success",
                                IsRead = false,
                                DisplayOrder = 0
                            };
                            _context.BillTrackings.Add(billTracking);
                        }
                    });
                    if (model.DepartmentId > 0)
                    {
                        var listUser = await _context.Users.Where(x => x.DepartmentId == model.DepartmentId && !x.IsDelete && x.Id != currentUser.Id).ToListAsync();
                        foreach (var userItem in listUser)
                        {
                            var billTracking = listbillTracking.FirstOrDefault(x => x.UserCode == userItem.Id.ToString());
                            if (billTracking == null)
                            {
                                billTracking = new BillTracking()
                                {
                                    BillId = 0,
                                    CustomerName = userItem.Id.ToString(),
                                    UserCode = userItem.Id.ToString(),
                                    UserIdReceived = model.Id,
                                    Note = "<strong>" + model.Name + "</strong><br/>" + fullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                                    TranType = TranTypeConst.SendToStaff,
                                    Status = "Success",
                                    IsRead = false,
                                    DisplayOrder = 0
                                };
                                _context.BillTrackings.Add(billTracking);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                try
                {
                    await _hubContext.Clients.All.BroadcastMessage();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("200") || ex.InnerException?.Message.Contains("200") == true)
                    {
                        // Lỗi không nghiêm trọng, có thể ghi log nếu cần
                        Console.WriteLine("BroadcastMessage lỗi nhẹ (bị bỏ qua): " + ex.Message);
                    }
                    else
                    {
                        return BadRequest(new { msg = ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
            return Ok(result);
        }
        else
            return NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userTaskService.Delete(id);
        return Ok(result);
    }

    [HttpPost("pintask/{id}")]
    public async Task<IActionResult> PinTask(int id)
    {
        await _userTaskService.PinTask(id, HttpContext.GetIdentityUser().Id);

        return Ok();
    }

    [HttpPost("statustask")]
    public async Task<IActionResult> StatusTask([FromBody] UserTaskStatusModel status)
    {
        var result = await _userTaskService.StatusTask(status, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _userTaskService.Get(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("getparentlist")]
    public async Task<IActionResult> GetParentList()
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var result = await _userTaskService.GetParentList(int.Parse(identity.FindFirst(x => x.Type == "UserId").Value));
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(result);
            }
            else
            {
                return BadRequest("Bạn không có quyền truy cập.");
            }
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetListMode([FromQuery] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetListMode(request, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("getduedate")]
    public async Task<IActionResult> GetDueDateMode([FromBody] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetDueDateMode(request, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetListMode(request, HttpContext.GetIdentityUser().Id);
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/DanhSachNhiemVu.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                sheet.DefaultColWidth = 15.0;
                int rowIdx = 5, startIndex = 5;

                sheet.Cells["A1"].Value = $"DANH SÁCH NHIỆM VỤ TỪ {request.StartDate.Value.ToString("dd/MM/yyyy")} ĐẾN {request.EndDate.Value.ToString("dd/MM/yyyy")}";
                sheet.Cells["A1"].Style.Font.Bold = true;

                foreach (var item in result.Data)
                {
                    sheet.Cells.Style.WrapText = true;
                    sheet.Cells[rowIdx, 1].Value = rowIdx - 4;
                    sheet.Cells[rowIdx, 2].Value = item.Name;
                    sheet.Cells[rowIdx, 3].Value = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : null;
                    sheet.Cells[rowIdx, 4].Value = item.DueDate.HasValue ? (item.DueDate.Value - Convert.ToDateTime(item.CreatedDate)).Days.ToString() : "Ko TH";
                    sheet.Cells[rowIdx, 5].Value = item.ActualHours;
                    sheet.Cells[rowIdx, 6].Value = item.CreatePerson;
                    sheet.Cells[rowIdx, 7].Value = string.Join("; ", item.ResponsiblePerson.Select(x => x.FullName).ToArray());
                    sheet.Cells[rowIdx, 8].Value = item.Project;
                    rowIdx++;
                }
                if (rowIdx > startIndex)
                {
                    // Assign borders
                    sheet.Cells[$"A{startIndex}:H{rowIdx - 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A{startIndex}:H{rowIdx - 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A{startIndex}:H{rowIdx - 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A{startIndex}:H{rowIdx - 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                package.SaveAs(stream);
            }

            stream.Seek(0L, SeekOrigin.Begin);
            string fileName = "DanhSachNhiemVu.xlsx";
            return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    [HttpPost("uploadfile")]
    public IActionResult UploadFile([FromForm] IFormFile file)
    {
        string fileType = System.IO.Path.GetExtension(file.FileName);
        string FileId = Guid.NewGuid().ToString();
        UserTaskFileModel fileUpload = new UserTaskFileModel()
        {
            FileId = FileId + fileType,
            FileName = file.FileName
        };
        return Ok(fileUpload);
    }

    [HttpPost("trackingtask")]
    public async Task<IActionResult> TrackingTask([FromBody] UserTaskTrackingModel request)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        var userTask = await _userTaskService.Get(request.UserTaskId, userId);
        var result = await _userTaskService.TrackingTask(request.UserTaskId, request.IsStart, userId, userTask.Status);
        return Ok(result);
    }

    [HttpGet("get-list-project")]
    public async Task<IActionResult> GetListTaskProject([FromQuery] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetListTaskProject(request, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpGet("get-list-project-parent")]
    public async Task<IActionResult> GetTaskProjectParent([FromQuery] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetTaskProjectParent(request, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpGet("get-list-project-children")]
    public async Task<IActionResult> GetTaskProjectChildren(int parentId, [FromQuery] UserTaskRequestModel request)
    {
        var result = await _userTaskService.GetTaskProjectChildren(HttpContext.GetIdentityUser().Id, parentId, request);
        return Ok(result);
    }

    [HttpPut("change-status-for-manager")]
    public async Task<IActionResult> ChangeStatusTaskForManager(int id, int status)
    {
        await _userTaskService.ChangeStatusTaskForManager(id, status, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
}