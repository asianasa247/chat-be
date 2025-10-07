using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Security.Claims;
using Common.Constants;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IUserSalaryService _userSalaryService;
    private readonly IUserContractService _userContractService;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        IFileService fileService,
        IUserSalaryService userSalaryService,
        IUserContractService userContractService
        ) 
    {
        _userService = userService;
        _mapper = mapper;
        _fileService = fileService;
        _userSalaryService = userSalaryService;
        _userContractService = userContractService;
    }
    //Kinh123456

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserViewModel param)
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        return Ok(await _userService.GetPaging(new UserMapper.FilterParams()
        {
            BirthDay = param.Birthday,
            Gender = param.Gender,
            Keyword = param.SearchText,
            PositionId = param.Positionid,
            WarehouseId = param.Warehouseid,
            DepartmentId = param.Departmentid,
            RequestPassword = param.RequestPassword,
            Quit = param.Quit,
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            StartDate = param.StartDate,
            EndDate = param.EndDate,
            TargetId = param.Targetid,
            Month = param.Month,
            DegreeId = param.Degreeid ?? 0,
            CertificateId = param.Certificateid ?? 0,
            Ids = param.Ids,
            roles = listRole,
            UserId = userId
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        UserModel result = null;
        var user = await _userService.GetByIdAsync(id);
        if (user != null)
        {
            result = _mapper.Map<UserModel>(user);
        }
        return Ok(result);
    }

    [HttpGet("get-total-reset-pass")]
    public async Task<IActionResult> GetTotalResetPass()
    {
        var total = await _userService.GetMany(x => !x.IsDelete && x.RequestPassword);
        return Ok(total.Count());
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UserModel model, [FromHeader] int yearFilter, [FromHeader] string dbName, int id = 0)
    {
        // map model to entity and set id
        model.YearCurrent = yearFilter;

        //if (HttpContext.User.Identity is ClaimsIdentity identity)
        //{
        //    user.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        //    user.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        //}

        if (id > 0)
        {
            await _userService.Update(model, dbName);
            return Ok();
        }
        else
        {
            await _userService.Create(model, "123456", dbName);
            return Ok();
        }
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] List<int> ids)
    {
        await _userService.ResetPasswordForMultipleUser(ids);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.Delete(id);
        return Ok();
    }

    [HttpPut("uploadAvatar/{id}")]
    public async Task<IActionResult> UploadAvatar(int id, [FromForm] IFormFile file)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user != null && !user.IsDelete)
            {
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    _fileService.DeleteFileUpload(user.Avatar);
                }
                var userModel = _mapper.Map<UserModel>(user);
                var fileName = _fileService.Upload(file, "user_avatar");
                userModel.Avatar = fileName;

                await _userService.Update(userModel);
                return Ok(new
                {
                    id = id,
                    Avatar = fileName
                });
            }
            else
            {
                return BadRequest(new { msg = ResultErrorConstants.USER_EMPTY_OR_DELETE });
            }
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    
    [HttpPost("upload-face-image")]
    public IActionResult UploadImage([FromForm] IFormFile file)
    {
        var response = _userService.UploadFile(file, AppConstant.FACE_DETECTOR_IMGS_FOLDER, file.FileName);
        return Ok(response);
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] List<int> ids, [FromQuery] bool allowImages = false)
    {
        if (ids == null || ids.Count == 0)
        {
            ids = new List<int>();
        }
        
        var identityUser = HttpContext.GetIdentityUser();
        
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(identityUser.Role);

        string filePath = await _userService.ExportExcel(ids, identityUser.Id, listRole, allowImages);

        if (string.IsNullOrEmpty(filePath))
        {
            return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        }
        
        return Ok(new BaseResponseModel
        {
            Data = filePath
        });
    }

    [HttpPost("getallusername")]
    public async Task<IActionResult> GetAllUserName()
    {
        var usernames = await _userService.GetAllUserName();
        return Ok(usernames);
    }

    [HttpGet("getAllUserActive")]
    public async Task<IActionResult> GetAllUserActive()
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var result = await _userService.GetAllUserActive(listRole, userId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("getAllUserActive1")]
    public async Task<IActionResult> GetAllUserActive1()
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var result = await _userService.GetAllUserActive1(listRole, userId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("UserStatistics")]
    public async Task<IActionResult> GetUserStatistics()
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);
       
        var result = await _userService.GetUserStatistics(listRole, userId);

        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("GetListUserSalary")]
    public async Task<IActionResult> GetListUserSalary(int month, int isInternal = 1)
    {
        var result = await _userSalaryService.GetListUserSalary(month, isInternal);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("GetListSalarySocial")]
    public async Task<IActionResult> GetListSalarySocial()
    {
        var result = await _userService.GetListSalarySocial();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
    [HttpGet("get-salarey-social-by-id/{id}")]
    public async Task<IActionResult> GetSalarySocialById([FromHeader] int yearFilter, int id)
    {
        var result = await _userService.GetSalarySocialById(id, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("exportUserSalary")]
    public async Task<IActionResult> ExportUserSalary(int month, int isInternal = 1)
    {
        var result = await _userSalaryService.ExportSalary(month, isInternal);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("HeaderThongKeTongQuat")]
    public async Task<IActionResult> HeaderThongKeTongQuat()
    {
        var result = await _userService.HeaderThongKeTongQuat();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("getListThongKeTongQuat")]
    public async Task<IActionResult> getListThongKeTongQuat()
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);


        var result = await _userService.GetListThongKeTongQuat(listRole, userId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpPost("UpdateSalarySocial")]
    public async Task<IActionResult> UpdateSalarySocial([FromBody] SalarySocial data)
    {
        await _userService.UpdateSalarySocial(data);
        return Ok();
    }

    [HttpGet("exportThongKeTongQuat")]
    public async Task<IActionResult> ExportThongKeTongQuat()
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        List<ThongKeTongQuat> datas = await _userService.GetListThongKeTongQuat(listRole, userId);
        List<SelectListModel> headers = await _userService.HeaderThongKeTongQuat();
        if (datas.Any())
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/ThongKeTongQuat.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                    int rowIdx = 7, nRowBegin = 7;
                    int nCol = 2;
                    foreach (var header in headers)
                    {
                        sheet.Cells[rowIdx, nCol].Value = header.Name;
                        nCol++;
                    }
                    rowIdx++;

                    foreach (ThongKeTongQuat lo in datas)
                    {
                        sheet.Cells[rowIdx, 1].Value = lo.Name;

                        nCol = 2;
                        foreach (var chiPhi in lo.listChildren)
                        {
                            sheet.Cells[rowIdx, nCol].Value = chiPhi.SoLuong;
                            nCol++;
                        }
                        rowIdx++;
                    }
                    rowIdx--;
                    nCol--;
                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 2, rowIdx, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    package.SaveAs(stream);
                }
            }
            stream.Seek(0L, SeekOrigin.Begin);
            string fileName = string.Format("ThongKeTongQuat_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        else
        {
            return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        }
    }

    [HttpGet("get-user-name")]
    public async Task<IActionResult> GetUserName()
    {
        string result = await _userService.GetUserName();
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPost("savelist")]
    public async  Task<IActionResult> SaveListUser([FromBody] List<UserModel> model)
    {
        var identityUser = HttpContext.GetIdentityUser();
        // map model to entity and set id
        if (model != null && model.Any())
        {
            await _userService.CreateExcel(model, identityUser.Id);
        }
        return Ok();
    }

    [HttpGet("UpdateSalaryToAccountant")]
    public async Task<IActionResult> UpdateSalaryToAccountant([FromHeader] int yearFilter, [FromQuery] int month, [FromQuery] int isInternal = 1)
    {
        await _userSalaryService.UpdateSalaryToAccountant(month, isInternal, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = null,
        });
    }
    [HttpPut("update-current-year")]
    public async Task<IActionResult> UpdateCurrentYear(int year)
    {
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }

        await _userService.UpdateCurrentYear(year, userId);
        return Ok(new BaseResponseModel
        {
            Data = null,
        });
    }
    [HttpGet("get-year-sale")]
    public async Task<IActionResult> GetYearSales()
    {
       var response =  await _userService.GetYearSales();
        return Ok(new BaseResponseModel
        {
            Data = response,
        });
    }

    [HttpGet("contract-labor/{userId}")]
    public async Task<IActionResult> LaborContractUser(int userId, int contractTypeId)
    {
        var fileName = await _userContractService.DownloadContract(userId, contractTypeId);
        return Ok(new BaseResponseModel
        {
            Data = fileName,
        });
    }

    [HttpGet("user-not-roles")]
    public async Task<IActionResult> GetAllUserNotRole()
    {
        var response = await _userService.GetAllUserNotRole();
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }
}