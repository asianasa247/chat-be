using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedureExportHelper : IProcedureExportHelper
{
    private readonly AppSettings _appSettings;
    private readonly ApplicationDbContext _context;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICompanyService _companyService;

    public ProcedureExportHelper(IOptions<AppSettings> appSettings,
        ApplicationDbContext context,
        IProcedureHelperService procedureHelperService,
        ICompanyService companyService)
    {
        _appSettings = appSettings.Value;
        _context = context;
        _procedureHelperService = procedureHelperService;
        _companyService = companyService;
    }

    public async Task<string> SignPlaceOrder(int id, string procedureCode,bool IsMaxcolumn = false)
    {
        var userIds = await _procedureHelperService.GetLogStep(procedureCode, id);
        var users = await _context.Users.Where(x => userIds.Contains(x.Id))
            .Select(x => new User
            {
                Id = x.Id,
                PositionDetailId = x.PositionDetailId,
                FullName = x.FullName,
                SignFile = x.SignFile,
            }).ToListAsync();
        var positionDetails = await _context.PositionDetails.Select(x => new PositionDetail
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();

        int index = 0;
        string _signtxt = "";
        if (IsMaxcolumn)
        {
            _signtxt = @"<td class='td-no-border'>{{{ROW1}}}</td>
                                <td class='td-no-border'>{{{ROW2}}}</td>
                                <td class='td-no-border'>{{{ROW3}}}</td>";
        }
        else
        {
            _signtxt = @"<td class='td-no-border'>{{{ROW1}}}</td>
                                <td class='td-no-border'>{{{ROW2}}}</td>
                                <td class='td-no-border'>{{{ROW3}}}</td>
                                <td class='td-no-border'>{{{ROW4}}}</td>";
        }
        foreach (var userId in userIds)
        {
            if (IsMaxcolumn)
            {
                if (index >= 3)
                {
                    break;
                }

            }

            var userPosition = "Người lập biểu";
            var _signtxtItem = @"<div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <img src='{{{UserSign}}}' height='100' width='100' alt='LoGo'/><br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>";
            var user = users.FirstOrDefault(x => x.Id == userId);

            if (index > 0)
            {
                userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;
            }

            if (user is null)
            {
                continue;
            }

            _signtxtItem = _signtxtItem
                .Replace("{{{UserPosition}}}", userPosition)
                .Replace("{{{UserSign}}}", $"{_appSettings.UrlHost}{user.SignFile}")
                .Replace("{{{UserName}}}", user.FullName);

            var rowindex = $"ROW{index + 1}";
            _signtxt =_signtxt.Replace("{{{" + rowindex + "}}}",_signtxtItem);
            index++;
        }
        _signtxt = _signtxt.Replace("{{{ROW1}}}", "")
                        .Replace("{{{ROW2}}}", "")
                        .Replace("{{{ROW3}}}", "")
                        .Replace("{{{ROW4}}}", "");
        return _signtxt;
    }

    public async Task<string> SignPlace(int id, string procedureCode)
    {
        var userIds = await _procedureHelperService.GetLogStep(procedureCode, id);
        var users = await _context.Users.Where(x => userIds.Contains(x.Id))
            .Select(x => new User
            {
                Id = x.Id,
                PositionDetailId = x.PositionDetailId,
                FullName = x.FullName,
                SignFile = x.SignFile,
            }).ToListAsync();
        var positionDetails = await _context.PositionDetails.Select(x => new PositionDetail
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();

        int index = 0;
        string _signtxt = @"<tr class='td-no-border' style='background-color: #fff;'>";
        foreach (var userId in userIds)
        {

            var userPosition = "Người lập biểu";
            var _signtxtItem = @"<td class='td-no-border'>
                                        <div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <img src='{{{UserSign}}}' height='100' alt='LoGo'/><br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>
                                    </td>";
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (user is null)
            {
                continue;
            }
            if (index > 0)
            {
                userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;
            }
            if (index == 4)
            {
                _signtxt += @"
                                    </tr>
                                    <tr class='td-no-border' style='background-color: #fff;'>";
            }
            _signtxtItem = _signtxtItem
                .Replace("{{{UserPosition}}}", userPosition)
                .Replace("{{{UserSign}}}", $"{_appSettings.UrlHost}{user.SignFile}")
                .Replace("{{{UserName}}}", user.FullName);

            _signtxt += _signtxtItem;
            index++;
        }
        
        _signtxt += "</tr>";

        return _signtxt;
    }

    public async Task<string> SignPlaceSameTr(int id, string procedureCode)
    {
        var userIds = await _procedureHelperService.GetLogStep(procedureCode, id);
        var users = await _context.Users.Where(x => userIds.Contains(x.Id))
            .Select(x => new User
            {
                Id = x.Id,
                PositionDetailId = x.PositionDetailId,
                FullName = x.FullName,
                SignFile = x.SignFile,
            }).ToListAsync();
        var positionDetails = await _context.PositionDetails.Select(x => new PositionDetail
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();

        int index = 0;
        string _signtxt = "";
        foreach (var userId in userIds)
        {

            var userPosition = "Người lập biểu";
            var _signtxtItem = @"<td class='td-no-border'>
                                        <div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <img src='{{{UserSign}}}' height='100' alt='LoGo'/><br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>
                                    </td>";
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (user is null)
            {
                continue;
            }
            if (index > 0)
            {
                userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;
            }
            
            _signtxtItem = _signtxtItem
                .Replace("{{{UserPosition}}}", userPosition)
                .Replace("{{{UserSign}}}", $"{_appSettings.UrlHost}{user.SignFile}")
                .Replace("{{{UserName}}}", user.FullName);

            _signtxt += _signtxtItem;
            index++;
        }

        return _signtxt;
    }


    public async Task<string> SignPlaceLastest(int id, string procedureCode)
    {
        var log = await _procedureHelperService.GetProcedureLog(procedureCode, id).LastOrDefaultAsync();
        var userId = log.UserId;
        var user = await _context.Users.Where(x => x.Id == userId)
            .Select(x => new User
            {
                Id = x.Id,
                PositionDetailId = x.PositionDetailId,
                FullName = x.FullName,
                SignFile = x.SignFile,
            }).FirstOrDefaultAsync();
        var positionDetails = await _context.PositionDetails.Where(x => x.Id == user.PositionDetailId).Select(x => new PositionDetail
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();

        var _signtxt = @"<div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <img src='{{{UserSign}}}' height='100' width='100' alt='LoGo'/><br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>";

        var userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;

        _signtxt = _signtxt
            .Replace("{{{UserPosition}}}", userPosition)
            .Replace("{{{UserSign}}}", $"{_appSettings.UrlHost}{user.SignFile}")
            .Replace("{{{UserName}}}", user.FullName);

        return _signtxt;
    }

    public async Task<string> SignPlaceLastestOrder(int id, string procedureCode)
    {
        var userIds = await _procedureHelperService.GetLogStep(procedureCode, id);

        if (userIds.Count() < 4)
        {
            var _signtxt = @"<div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'></p><br/>
                                            <div height='100' width='100'></div><br/>
                                            <p class='signature-item-signer'></p><br/>
                                        </div>";
            var company = await _companyService.GetCompany();
            _signtxt = _signtxt.Replace("{{{UserPosition}}}", company.NoteOfCEO);
            return _signtxt;
        }
        else
        {
            var _signtxt = @"<div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <img src='{{{UserSign}}}' height='100' width='100' alt='LoGo'/><br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>";
            var userId = userIds.Last();
            var user = await _context.Users.Where(x => x.Id == userId)
                .Select(x => new User
                {
                    Id = x.Id,
                    PositionDetailId = x.PositionDetailId,
                    FullName = x.FullName,
                    SignFile = x.SignFile,
                }).FirstOrDefaultAsync();
            var positionDetails = await _context.PositionDetails.Where(x => x.Id == user.PositionDetailId).Select(x => new PositionDetail
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            var userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;

            _signtxt = _signtxt
                .Replace("{{{UserPosition}}}", userPosition)
                .Replace("{{{UserSign}}}", $"{_appSettings.UrlHost}{user.SignFile}")
                .Replace("{{{UserName}}}", user.FullName);
            return _signtxt;
        }

    }
}