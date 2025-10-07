using AutoMapper;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services.P_ProcedureServices.ExpenditurePlanServices;
public class ExpenditurePlanService : IExpenditurePlanService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public ExpenditurePlanService(ApplicationDbContext context, 
        IMapper mapper,
        IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }
    public async Task<PagingResult<ExpenditurePlanPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.ExpenditurePlans
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));
        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPENDITURE_PLAN));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.EXPENDITURE_PLAN));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));

        if (param.StatusTab != ProduceProductStatusTab.Pending)
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.EXPENDITURE_PLAN) && x.log.UserId == userId
            && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        switch (param.StatusTab)
        {
            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsPart && !x.IsDone);
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
        }

        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var datas = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var listOut = new List<ExpenditurePlanPagingModel>();
        foreach (var item in datas)
        {
            var itemOut = _mapper.Map<ExpenditurePlanPagingModel>(item);
            itemOut.ApproveAmount = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == itemOut.Id).SumAsync(x => x.ApproveAmount);
            itemOut.ExpenditurePlanAmount = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == itemOut.Id).SumAsync(x => x.ExpenditurePlanAmount);

            itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;
            if (item.IsFinished)
            {
                item.ProcedureNumber = item.Code;
            }
            listOut.Add(itemOut);
        }
        var totalItem = await query.CountAsync();
        return new PagingResult<ExpenditurePlanPagingModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ExpenditurePlanModel> GetDetail(int id)
    {
        var item = await _context.ExpenditurePlans.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<ExpenditurePlanModel>(item);

        itemOut.Items = new List<ExpenditurePlanDetailModel>();
        var details = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == id).ToListAsync();
        foreach (var detail in details)
        {
            var itemDetail = _mapper.Map<ExpenditurePlanDetailModel>(detail);
            if (!string.IsNullOrEmpty(detail.FileStr))
            {
                itemDetail.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(detail.FileStr);
            }
            if (!string.IsNullOrEmpty(detail.FileStatusStr))
            {
                itemDetail.FileStatuses = JsonConvert.DeserializeObject<List<FileDetailModel>>(detail.FileStatusStr);
            }
            itemOut.Items.Add(itemDetail);
        }

        return itemOut;
    }

    public async Task Create(ExpenditurePlanSetterModel form, int userId)
    {

        var procedure = new ExpenditurePlan()
        {
            ProcedureNumber = await GetProcedureNumber(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserCreated = userId,
            UserId = userId,
            UserUpdated = userId,
            IsFinished = false,
            Date = DateTime.Now,
            Note = form.Note,
        };

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPENDITURE_PLAN));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;
        await _context.ExpenditurePlans.AddAsync(procedure);
        await _context.SaveChangesAsync();

        await AddDetail(form, procedure.Id);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.EXPENDITURE_PLAN), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Update(ExpenditurePlanSetterModel form, int userId)
    {
        var procedure = await _context.ExpenditurePlans.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (procedure.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        procedure.Note = form.Note;

        _context.ExpenditurePlans.Update(procedure);
        await _context.SaveChangesAsync();
        await AddDetail(form, procedure.Id);
    }

    private async Task AddDetail(ExpenditurePlanSetterModel form, int id)
    {

        var itemAdds = new List<ExpenditurePlanDetail>();

        if (form.PaymentProposalIds != null && form.PaymentProposalIds.Any())
        {

            var paymentProposalDetails = await _context.PaymentProposalDetails.Where(x => form.PaymentProposalIds.Contains(x.PaymentProposalId)).ToListAsync();
            var paymentProposals = await _context.PaymentProposals.Where(x => form.PaymentProposalIds.Contains(x.Id)).ToListAsync();
            if (paymentProposals.Any(x => !x.IsFinished))
            {
                throw new ErrorException(ErrorMessages.ProcedureNotFinished);
            }
            foreach (var paymentProposal in paymentProposals)
            {
                var item = new ExpenditurePlanDetail
                {
                    ExpenditurePlanId = id,
                    ApproveAmount = paymentProposalDetails.Where(x => x.PaymentProposalId == paymentProposal.Id).Sum(x => x.Amount),
                    ExpenditurePlanAmount = paymentProposalDetails.Where(x => x.PaymentProposalId == paymentProposal.Id).Sum(x => x.Amount),
                    CreatedAt = DateTime.Now,
                    FromProcedureId = paymentProposal.Id,
                    FromProcedureCode = paymentProposal.Code,
                    FromProcedureNote = paymentProposal.Note,
                    IsApply = true,
                    FromTableName = nameof(PaymentProposal),
                    Status = nameof(ExpenditurePlanStatusEnum.Unspent),
                };
                itemAdds.Add(item);
                paymentProposal.IsInprogress = true;
            }
            _context.PaymentProposals.UpdateRange(paymentProposals);

        }
        if (form.AdvancePaymentIds != null && form.AdvancePaymentIds.Any())
        {

            var advancePaymentDetails = await _context.AdvancePaymentDetails.Where(x => form.AdvancePaymentIds.Contains(x.AdvancePaymentId)).ToListAsync();
            var advancePayments = await _context.AdvancePayments.Where(x => form.AdvancePaymentIds.Contains(x.Id)).ToListAsync();
            if (advancePayments.Any(x => !x.IsFinished))
            {
                throw new ErrorException(ErrorMessages.ProcedureNotFinished);
            }
            foreach (var advancePayment in advancePayments)
            {
                var item = new ExpenditurePlanDetail
                {
                    ExpenditurePlanId = id,
                    ApproveAmount = advancePaymentDetails.Where(x => x.AdvancePaymentId == advancePayment.Id).Sum(x => x.Amount),
                    ExpenditurePlanAmount = advancePaymentDetails.Where(x => x.AdvancePaymentId == advancePayment.Id).Sum(x => x.Amount),
                    CreatedAt = DateTime.Now,
                    FromProcedureId = advancePayment.Id,
                    FromProcedureCode = advancePayment.Code,
                    FromProcedureNote = advancePayment.Note,
                    IsApply = true,
                    FromTableName = nameof(AdvancePayment),
                    Status = nameof(ExpenditurePlanStatusEnum.Unspent),
                };
                itemAdds.Add(item);
            }
        }
        await _context.ExpenditurePlanDetails.AddRangeAsync(itemAdds);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateExpenditure(ExpenditurePlanModel form, int userId)
    {
        var procedure = await _context.ExpenditurePlans.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        procedure.Note = form.Note;

        if (procedure.IsFinished)
        {
            var details = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == form.Id).ToListAsync();
            var detailUpdateIds = form.Items.Select(X => X.Id);
            var detailRemoves = details.Any(x => !detailUpdateIds.Contains(x.Id));
            if (detailRemoves)
            {
                throw new ErrorException(ErrorMessages.CannotRemoveDetailApplied);
            }

            foreach (var update in details)
            {
                var item = form.Items.Find(x => x.Id == update.Id);
                if (!string.IsNullOrEmpty(update.FileStatusStr) && item.FileStatuses == null)
                {
                    throw new ErrorException(ErrorMessages.CannotRemoveDetailApplied);
                }

                if (item.FileStatuses != null)
                {
                    update.FileStatusStr = JsonConvert.SerializeObject(item.FileStatuses);
                }
                update.Status = item.Status;
            }
            procedure.IsDone = details.All(x => !string.IsNullOrEmpty(x.FileStatusStr));
            procedure.IsPart = details.Any(x => !string.IsNullOrEmpty(x.FileStatusStr)) && details.Any(x => string.IsNullOrEmpty(x.FileStatusStr));
            var paymentProposalIds = details.Where(x => x.FromTableName == nameof(PaymentProposal)).Select(x => x.FromProcedureId);
            if (paymentProposalIds.Any())
            {
                var paymentProposals = await _context.PaymentProposals.Where(x => paymentProposalIds.Contains(x.Id)).ToListAsync();
                foreach(var paymentProposal in paymentProposals)
                {
                    var expenditurePlanDetail =
                    paymentProposal.IsInprogress = details.FirstOrDefault(x => x.FromProcedureId == paymentProposal.Id)?.Status == nameof(ExpenditurePlanStatusEnum.Unspent);
                    paymentProposal.IsPart = details.FirstOrDefault(x => x.FromProcedureId == paymentProposal.Id)?.Status == nameof(ExpenditurePlanStatusEnum.PartiallyPaid);
                    paymentProposal.IsDone = details.FirstOrDefault(x => x.FromProcedureId == paymentProposal.Id)?.Status == nameof(ExpenditurePlanStatusEnum.FullyPaid);
                }   
                _context.PaymentProposals.UpdateRange(paymentProposals);
            }

            _context.ExpenditurePlanDetails.UpdateRange(details);
        }
        else
        {
            var details = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == form.Id).ToListAsync();
            var detailUpdateIds = form.Items.Select(X => X.Id);
            var detailRemoves = details.Where(x => !detailUpdateIds.Contains(x.Id)).ToList();
            if (detailRemoves.Any(x => x.IsApply))
            {
                throw new ErrorException(ErrorMessages.CannotRemoveDetailApplied);
            }

            _context.ExpenditurePlanDetails.RemoveRange(detailRemoves);

            var detailUpdates = details.Where(x => detailUpdateIds.Contains(x.Id)).ToList();
            foreach (var update in detailUpdates)
            {
                var item = form.Items.Find(x => x.Id == update.Id);
                update.Note = item.Note;
                update.ApproveAmount = item.ApproveAmount;
                update.ExpenditurePlanAmount = item.ExpenditurePlanAmount;
                update.IsApply = item.IsApply;

                if (!string.IsNullOrEmpty(update.FileStr) && item.Files == null)
                {
                    throw new ErrorException(ErrorMessages.CannotRemoveDetailApplied);
                }

                if (item.Files != null)
                {
                    update.FileStr = JsonConvert.SerializeObject(item.Files);
                }
            }
            _context.ExpenditurePlanDetails.UpdateRange(detailUpdates);

        }

        _context.ExpenditurePlans.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.ExpenditurePlans.FindAsync(id);
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

        _context.ExpenditurePlans.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.EXPENDITURE_PLAN), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.EXPENDITURE_PLAN), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.ExpenditurePlans.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPENDITURE_PLAN));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.ExpenditurePlans.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.EXPENDITURE_PLAN), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var item = await _context.ExpenditurePlans.FindAsync(id);
        if (item != null)
        {
            _context.ExpenditurePlans.Remove(item);
            var details = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == id).ToListAsync();
            _context.ExpenditurePlanDetails.RemoveRange(details);
            await _context.SaveChangesAsync();
            await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.EXPENDITURE_PLAN));

        }
    }

    private async Task<string> GetProcedureNumber()
    {
        var item = await _context.ExpenditurePlans.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.EXPENDITURE_PLAN)}-{procedureNumber}";
    }

    public async Task<IEnumerable<ExpenditurePlanGetListModel>> GetList()
    {
        return await _context.ExpenditurePlans.Where(x => !x.IsFinished).Select(x => _mapper.Map<ExpenditurePlanGetListModel>(x)).ToListAsync();
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.ExpenditurePlans.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "DC");
    }
}
