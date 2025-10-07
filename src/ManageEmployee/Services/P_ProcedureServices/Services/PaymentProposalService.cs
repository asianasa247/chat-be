using System.Text;
using AutoMapper;
using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageEmployee.Services.P_ProcedureServices.Services;

public class PaymentProposalService : IPaymentProposalService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;
    public PaymentProposalService(ApplicationDbContext context, IMapper mapper,
        IProcedureHelperService procedureHelperService,
        ICompanyService companyService,
        IConverter converterPDF,
        IOptions<AppSettings> appSettings,
        IProcedureExportHelper procedureExportHelper)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<PagingResult<PaymentProposalPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.PaymentProposals
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText))
                    .Where(x => x.TableId == null);

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PAYMENT_PROPOSAL));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.PAYMENT_PROPOSAL));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId)
        || param.StatusTab == ProduceProductStatusTab.Pending && x.UserId == userId && x.ProcedureStatusId == startStatus.Id);

        if (param.StatusTab == ProduceProductStatusTab.Approved)
        {
            query = query
            .Join(_context.ProcedureLogs,
                    b => b.Id,
                    d => d.ProcedureId,
                    (b, d) => new
                    {
                        procedure = b,
                        log = d
                    })
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.PAYMENT_PROPOSAL) && x.log.UserId == userId
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        switch (param.StatusTab)
        {
            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsPart);
                break;
            case ProduceProductStatusTab.Finish:
                query = query.Where(x => x.IsDone);
                break;
            case ProduceProductStatusTab.Part:
                query = query.Where(x => x.IsPart);
                break;
            case ProduceProductStatusTab.Approved:
                query = query.Where(x => !x.IsFinished);
                break;
            case ProduceProductStatusTab.InProgess:
                query = query.Where(x => x.IsInprogress);
                break;
        }


        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var itemOuts = new List<PaymentProposalPagingModel>();
        foreach (var item in data)
        {
            var itemOut = _mapper.Map<PaymentProposalPagingModel>(item);
            itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;
            if (!string.IsNullOrEmpty(item.FileStr))
            {
                itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
            }
            itemOut.UserName = await _context.Users.Where(x => x.Id == item.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (item.IsFinished)
            {
                itemOut.ProcedureNumber = item.Code;
            }
            itemOuts.Add(itemOut);
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<PaymentProposalPagingModel>
        {
            Data = itemOuts,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<PaymentProposalModel> GetDetail(int id)
    {
        var item = await _context.PaymentProposals.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null)
        {
            return new PaymentProposalModel();
        }
        var itemOut = _mapper.Map<PaymentProposalModel>(item);
        if (!string.IsNullOrEmpty(item.FileStr))
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
        }

        itemOut.Items = await _context.PaymentProposalDetails.Where(X => X.PaymentProposalId == id).Select(x => _mapper.Map<PaymentProposalDetailModel>(x)).ToListAsync();
        return itemOut;
    }

    public async Task Create(PaymentProposalModel form, int userId, int? tableId = null, string tableName = null)
    {
        var procedure = _mapper.Map<PaymentProposal>(form);
        procedure.TableName = tableName;
        procedure.TableId = tableId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PAYMENT_PROPOSAL));

        if (tableId == null)
        {
            procedure.ProcedureStatusId = status.Id;
            procedure.ProcedureStatusName = status.P_StatusName;
        }

        procedure.AdvancePaymentId = form.AdvancePaymentId;
        procedure.RequestEquipmentId = form.RequestEquipmentId;
        procedure.RequestEquipmentOrderId = form.RequestEquipmentOrderId;
        procedure.AdvancePaymentCode = await _context.AdvancePayments.Where(x => x.Id == form.AdvancePaymentId).Select(x => x.Code).FirstOrDefaultAsync();
        procedure.RequestEquipmentCode = await _context.RequestEquipments.Where(x => x.Id == form.RequestEquipmentId).Select(x => x.Code).FirstOrDefaultAsync();
        procedure.RequestEquipmentOrderCode = await _context.RequestEquipmentOrders.Where(x => x.Id == form.RequestEquipmentOrderId).Select(x => x.Code).FirstOrDefaultAsync();

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        await _context.PaymentProposals.AddAsync(procedure);
        await _context.SaveChangesAsync();

        var itemAdds = form.Items.Select(x => new PaymentProposalDetail
        {
            PaymentProposalId = procedure.Id,
            Amount = x.Amount,
            Content = x.Content,
            Note = x.Note,
        }).ToList();
        await _context.PaymentProposalDetails.AddRangeAsync(itemAdds);
        await _context.SaveChangesAsync();

        if (tableId == null)
        {
            await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PAYMENT_PROPOSAL), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
        }
    }

    public async Task Update(PaymentProposalModel form, int userId)
    {
        var procedure = await _context.PaymentProposals.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        procedure.UserId = form.UserId;
        procedure.Note = form.Note;
        procedure.IsImmediate = form.IsImmediate;
        procedure.Date = form.Date ?? DateTime.Now;
        procedure.PaymentMethod = form.PaymentMethod;
        procedure.AdvanceAmount = form.AdvanceAmount;
        procedure.RefundAmount = form.RefundAmount;
        procedure.TotalAmount = form.TotalAmount;

        procedure.AdvancePaymentId = form.AdvancePaymentId;
        procedure.RequestEquipmentId = form.RequestEquipmentId;
        procedure.RequestEquipmentOrderId = form.RequestEquipmentOrderId;
        procedure.AdvancePaymentCode = await _context.AdvancePayments.Where(x => x.Id == form.AdvancePaymentId).Select(x => x.Code).FirstOrDefaultAsync();
        procedure.RequestEquipmentCode = await _context.RequestEquipments.Where(x => x.Id == form.RequestEquipmentId).Select(x => x.Code).FirstOrDefaultAsync();
        procedure.RequestEquipmentOrderCode = await _context.RequestEquipmentOrders.Where(x => x.Id == form.RequestEquipmentOrderId).Select(x => x.Code).FirstOrDefaultAsync();

        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.PaymentProposals.Update(procedure);

        var itemDels = await _context.PaymentProposalDetails.Where(X => X.PaymentProposalId == form.Id).ToListAsync();
        _context.PaymentProposalDetails.RemoveRange(itemDels);

        var itemAdds = form.Items.Select(x => new PaymentProposalDetail
        {
            PaymentProposalId = procedure.Id,
            Amount = x.Amount,
            Content = x.Content,
            Note = x.Note,
        }).ToList();
        await _context.PaymentProposalDetails.AddRangeAsync(itemAdds);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.PaymentProposals.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            procedure.IsFinished = status.IsFinish;
            procedure.Code = await GetCodeAsync();
        }

        _context.PaymentProposals.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PAYMENT_PROPOSAL), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PAYMENT_PROPOSAL), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.PaymentProposals.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PAYMENT_PROPOSAL));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.PaymentProposals.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PAYMENT_PROPOSAL), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.PAYMENT_PROPOSAL));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        var item = await _context.PaymentProposals.FindAsync(id);
        if (item != null)
        {
            _context.PaymentProposals.Remove(item);
            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.PAYMENT_PROPOSAL));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.PaymentProposals.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.PAYMENT_PROPOSAL)}-{procedureNumber}";
    }

    public async Task<ProcedureCheckButton> CheckButton(int id, int userId)
    {
        var itemOut = new ProcedureCheckButton()
        {
            IsAdd = false,
            IsAccept = false,
            IsDelete = false,
            IsSave = false,
        };

        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.UserNotFound);
        }
        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();
        var step = new P_ProcedureStatusStep();

        if (id == 0)
        {
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PAYMENT_PROPOSAL));
            step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == status.Id);
            itemOut.IsAdd = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));
            return itemOut;
        }

        var procedure = await _context.AdvancePayments.FindAsync(id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        if (procedure.IsFinished)
        {
            return itemOut;
        }

        step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == procedure.ProcedureStatusId);

        if (step is null)
        {
            throw new ErrorException(ErrorMessages.ProcedureStepNotFound);
        }
        itemOut.IsDelete = step.IsInit;

        itemOut.IsSave = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        itemOut.IsAccept = procedure.UserId == userId && step.IsInit ||
                                await _context.P_ProcedureStatusRole
                                .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdTo
                                            && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        return itemOut;
    }

    public async Task<string> Export(int paymentProposalId, bool shouldExport = false)
    {
        var payment = await _context.PaymentProposals.FindAsync(paymentProposalId);
        if (!payment.IsFinished && !shouldExport)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var user = await _context.Users.FindAsync(payment.UserCreated);

        var userPosition = await _context.PositionDetails.FirstOrDefaultAsync(x => x.Id == user.PositionDetailId);

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var paymentDetails = await _context.PaymentProposalDetails.Where(x => x.PaymentProposalId == paymentProposalId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlace(paymentProposalId, nameof(ProcedureEnum.PAYMENT_PROPOSAL));

        var resultHtml = GenerateExportTable(paymentDetails, soThapPhan);

        var paymentDate = payment.Date;
        var totalAmount = paymentDetails.Sum(x => x.Amount);
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("Code", payment.Code),
            ("Note", payment.Note),
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("UserName", user?.FullName),
            ("UserPosition", userPosition?.Name),
            ("PaymentMethod", payment.PaymentMethod),
            ("TotalAmount", totalAmount.FormatAmount(soThapPhan)),
            ("TotalAmountText", totalAmount.ConvertFromDecimal()),
            ("IsImmediate",  payment.IsImmediate ? "checked" : string.Empty),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "PhieuDeNghiThanhToan.html"
        );

        var allTextBuilder = new StringBuilder(allText);

        values.ForEach(x =>
        {
            allTextBuilder.Replace("{{{" + x.FieldName + "}}}", x.FieldValue);
        });

        allTextBuilder.Replace("##SIGN_REPLACE_PLACE##", resultSignHtml);
        allTextBuilder.Replace("##REPLACE_PLACE##", resultHtml);

        return ExcelHelpers.ConvertUseDink(
            allTextBuilder.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "PhieuDeNghiThanhToan"
        );
    }

    private string GenerateExportTable(List<PaymentProposalDetail> paymentDetails, string soThapPhan)
    {
        var resultHtml = new StringBuilder();
        for (int i = 1; i <= paymentDetails.Count; i++)
        {
            var paymentDetail = paymentDetails[i - 1];
            resultHtml.Append(GenerateExportRow(i, paymentDetail, soThapPhan));
        }

        return resultHtml.ToString();
    }

    private static string GenerateExportRow(int rowIndex, PaymentProposalDetail paymentDetail, string soThapPhan)
    {
        string txt = $@"<tr>
                                <td class='txt-center'>{rowIndex}</td>
                                <td>{paymentDetail.Content}</td>
                                <td class='txt-right'>{paymentDetail.Amount.FormatAmount(soThapPhan)}</td>
                                <td>{paymentDetail.Note}</td>
                            </tr>";
        return txt;
    }

    public async Task ResetTableId(PaymentProposal data, int tableId)
    {
        data.TableId = tableId;
        _context.PaymentProposals.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PaymentProposal>> GetListForTableName(string tableName, IEnumerable<int> tableIds)
    {
        return await _context.PaymentProposals.Where(x => tableIds.Contains(x.TableId ?? 0) && x.TableName == tableName).ToListAsync();
    }

    public async Task SetForOtherTable(PaymentProposal data, int userId, int? tableId, string tableName)
    {
        var procedure = new PaymentProposal
        {
            TableId = tableId,
            TableName = tableName,
            CreatedAt = DateTime.Now,
            AdvanceAmount = data.AdvanceAmount,
            Date = data.Date,
            IsImmediate = data.IsImmediate,
            PaymentMethod = data.PaymentMethod,
            RefundAmount = data.RefundAmount,
            TotalAmount = data.TotalAmount,
            UpdatedAt = DateTime.Now,
            UserCreated = userId,
            Note = data.Note,
            UserUpdated = userId,
            UserId = userId,
            IsFinished = false,
        };
        await _context.PaymentProposals.AddAsync(procedure);
        await _context.SaveChangesAsync();

        var itemAdds = await _context.PaymentProposalDetails.Where(X => X.PaymentProposalId == data.Id).Select(x => new PaymentProposalDetail
        {
            PaymentProposalId = procedure.Id,
            Amount = x.Amount,
            Content = x.Content,
            Note = x.Note,
        }).ToListAsync();
        await _context.PaymentProposalDetails.AddRangeAsync(itemAdds);
        await _context.SaveChangesAsync();
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;
        var codeNumber = await _context.PaymentProposals.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "TT");
    }
}