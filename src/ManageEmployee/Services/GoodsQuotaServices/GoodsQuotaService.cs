using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsQuotaServices;
public class GoodsQuotaService : IGoodsQuotaService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProcedureHelperService _procedureHelperService;

    public GoodsQuotaService(ApplicationDbContext dbContext,
        IProcedureHelperService procedureHelperService)
    {
        _dbContext = dbContext;
        _procedureHelperService = procedureHelperService;
    }
    public async Task<PagingResult<GoodsQuotaPagingGetterModel>> GetPaging(GoodsQuotasRequestModel searchRequest, int userId)
    {
        var query = _dbContext.GoodsQuotas
                                     .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) || x.Name.ToLower().Contains(searchRequest.SearchText.ToLower()));
        /*
         * 
         */
        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GOOD_QUOTA));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, searchRequest.StatusTab, nameof(ProcedureEnum.GOOD_QUOTA));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));

        if (searchRequest.StatusTab == ProduceProductStatusTab.Approved)
        {
            query = query.Where(x => !x.IsFinished)
            .Join(_dbContext.ProcedureLogs,
                    b => b.Id,
                    d => d.ProcedureId,
                    (b, d) => new
                    {
                        procedure = b,
                        log = d
                    })
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.GOOD_QUOTA) && x.log.UserId == userId
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        if (searchRequest.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }


        var totalItem = 0;
        var goodsQuotas = new List<GoodsQuotaPagingGetterModel>();
        if (searchRequest.GoodsQuotaRecipeId == null)
        {
            var goodsQuotaRecipeIdAlls = await query.Select(x => x.GoodsQuotaRecipeId).Distinct().ToListAsync();
            var goodsQuotaRecipeIds = goodsQuotaRecipeIdAlls.Skip(searchRequest.Page * searchRequest.PageSize).Take(searchRequest.PageSize).ToList();
            totalItem = goodsQuotaRecipeIdAlls.Count();
            foreach (var goodsQuotaRecipeId in goodsQuotaRecipeIds)
            {
                var goodsQuotaAdd = await query.Where(x => x.GoodsQuotaRecipeId == goodsQuotaRecipeId).OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).FirstOrDefaultAsync();

                if (goodsQuotaAdd != null)
                {
                    goodsQuotas.Add(new GoodsQuotaPagingGetterModel
                    {
                        Id = goodsQuotaAdd.Id,
                        Name = goodsQuotaAdd.Name,
                        Date = goodsQuotaAdd.Date,
                        Code = goodsQuotaAdd.GoodsQuotaCode,
                        ProcedureStatusName = goodsQuotaAdd.ProcedureStatusName,
                        GoodsQuotaRecipeId = goodsQuotaAdd.GoodsQuotaRecipeId,
                    });
                }
            }
        }
        else
        {
            query = query.Where(x => x.GoodsQuotaRecipeId == searchRequest.GoodsQuotaRecipeId);
            totalItem = await query.CountAsync();
            goodsQuotas = await query.Skip(searchRequest.Page * searchRequest.PageSize).Take(searchRequest.PageSize)
                .Select(x => new GoodsQuotaPagingGetterModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Date = x.Date,
                    Code = x.GoodsQuotaCode,
                    ProcedureStatusName = x.ProcedureStatusName,
                    GoodsQuotaRecipeId = x.GoodsQuotaRecipeId,
                })
                .ToListAsync();
        }

        var goodsQuotaRecipes = await _dbContext.GoodsQuotaRecipes.ToListAsync();
        var goodsQuotaIds = goodsQuotas.Select(x => x.Id);
        var goodsQuotaItems = await _dbContext.GoodsQuotaDetails.Where(x => goodsQuotaIds.Contains(x.GoodsQuotaId)).ToListAsync();
        foreach (var goodsQuota in goodsQuotas)
        {
            goodsQuota.ItemNames = string.Join(", ", goodsQuotaItems.Where(x => x.GoodsQuotaId == goodsQuota.Id)
                                                                    .Select(x => !string.IsNullOrEmpty(x.DetailName2) ? x.DetailName2 : x.DetailName1));
            goodsQuota.ItemCodes = string.Join(", ", goodsQuotaItems.Where(x => x.GoodsQuotaId == goodsQuota.Id)
                                                                    .Select(x => !string.IsNullOrEmpty(x.Detail2) ? x.Detail2 : x.Detail1));
            goodsQuota.GoodsQuotaRecipeName = goodsQuotaRecipes.Find(x => x.Id == goodsQuota.GoodsQuotaRecipeId)?.Name;

            goodsQuota.ShoulNotAccept = searchRequest.StatusTab == ProduceProductStatusTab.Pending && goodsQuota.ProcedureStatusName != startStatus.P_StatusName;
        }

        return new PagingResult<GoodsQuotaPagingGetterModel>()
        {
            CurrentPage = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalItems = totalItem,
            Data = goodsQuotas
        };
    }

    public async Task<IEnumerable<GoodsQuotaGetAllModel>> GetAll()
    {
        return await _dbContext.GoodsQuotas.Where(x => x.IsFinished)
                                     .Select(x => new GoodsQuotaGetAllModel
                                     {
                                         Id = x.Id,
                                         Name = x.Name,
                                         Code = x.GoodsQuotaCode,
                                     }).ToListAsync();

    }

    public async Task<GoodsQuotaModel> GetDetail(int id)
    {
        var goods = await _dbContext.GoodsQuotas.FindAsync(id);
        var details = await _dbContext.GoodsQuotaDetails.Where(x => x.GoodsQuotaId == id).ToListAsync();
        return new GoodsQuotaModel
        {
            Date = goods.Date,
            Name = goods.Name,
            Code = goods.GoodsQuotaCode,
            GoodsQuotaRecipeId = goods.GoodsQuotaRecipeId,
            Id = id,
            Items = details.ConvertAll(X => new GoodsQuotaDetailModel
            {
                Quantity = X.Quantity,
                Account = X.Account,
                AccountName = X.AccountName,
                Detail1 = X.Detail1,
                Detail2 = X.Detail2,
                DetailName1 = X.DetailName1,
                DetailName2 = X.DetailName2,
                GoodsQuotaId = id,
                Warehouse = X.Warehouse,
            })
        };
    }
    public async Task Create(GoodsQuotaModel param, int userId)
    {
        var goodsQuota = new GoodsQuota
        {
            Date = param.Date,
            Name = param.Name,
            GoodsQuotaCode = param.Code,
            GoodsQuotaRecipeId = param.GoodsQuotaRecipeId,
            ProcedureNumber = param.ProcedureNumber,
            UserCreated = userId,
            CreatedAt = DateTime.Now,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
            IsFinished = false,
        };
        if (string.IsNullOrEmpty(param.ProcedureNumber))
        {
            goodsQuota.ProcedureNumber = await GetProcedureNumber();
        }
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GOOD_QUOTA));
        goodsQuota.ProcedureStatusId = status.Id;
        goodsQuota.ProcedureStatusName = status.P_StatusName;

        await _dbContext.GoodsQuotas.AddAsync(goodsQuota);
        await _dbContext.SaveChangesAsync();
        if (param.Items != null)
        {
            var goodsQuotaDetails = param.Items.Select(X => new GoodsQuotaDetail
            {
                Quantity = X.Quantity,
                Account = X.Account,
                AccountName = X.AccountName,
                Detail1 = X.Detail1,
                Detail2 = X.Detail2,
                DetailName1 = X.DetailName1,
                DetailName2 = X.DetailName2,
                GoodsQuotaId = goodsQuota.Id,
                Warehouse = X.Warehouse,
            });
            await _dbContext.GoodsQuotaDetails.AddRangeAsync(goodsQuotaDetails);
            await _dbContext.SaveChangesAsync();
        }
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.GOOD_QUOTA), status.Id, goodsQuota.Id, userId, goodsQuota.ProcedureNumber);

    }
    public async Task Update(GoodsQuotaModel param, int userId)
    {
        var goodsQuota = await _dbContext.GoodsQuotas.FindAsync(param.Id);
        goodsQuota.Date = param.Date;
        goodsQuota.Name = param.Name;
        goodsQuota.GoodsQuotaCode = param.Code;
        goodsQuota.GoodsQuotaRecipeId = param.GoodsQuotaRecipeId;
        goodsQuota.UpdatedAt = DateTime.Now;
        goodsQuota.UserCreated = userId;
        _dbContext.GoodsQuotas.Update(goodsQuota);

        var goodsQuotaDetailDeletes = await _dbContext.GoodsQuotaDetails.Where(x => x.GoodsQuotaId == param.Id).ToListAsync();
        _dbContext.GoodsQuotaDetails.RemoveRange(goodsQuotaDetailDeletes);
        if (param.Items != null)
        {
            var goodsQuotaDetails = param.Items.Select(X => new GoodsQuotaDetail
            {
                Quantity = X.Quantity,
                Account = X.Account,
                AccountName = X.AccountName,
                Detail1 = X.Detail1,
                Detail2 = X.Detail2,
                DetailName1 = X.DetailName1,
                DetailName2 = X.DetailName2,
                GoodsQuotaId = goodsQuota.Id,
                Warehouse = X.Warehouse,
            });
            await _dbContext.GoodsQuotaDetails.AddRangeAsync(goodsQuotaDetails);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var produce = await _dbContext.GoodsQuotas.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        // validate condition
        var status = await _procedureHelperService.GetStatusAccept(produce.ProcedureStatusId ?? 0, userId);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;


        if (status.IsFinish)
        {
            produce.IsFinished = status.IsFinish;
            produce.Code = await GetCodeAsync();
        }
        produce.UpdatedAt = DateTime.Now;

        _dbContext.GoodsQuotas.Update(produce);
        await _dbContext.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.GOOD_QUOTA), status.Id, userId, id, produce.ProcedureNumber);
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.GOOD_QUOTA), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _dbContext.GoodsQuotas.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GOOD_QUOTA));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _dbContext.GoodsQuotas.Update(produce);
        await _dbContext.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.GOOD_QUOTA), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task GoodsQuotaForGoodsDetail(List<int> goodIds, int goodsQuotaId)
    {
        var goodsQuotaDetails = await _dbContext.GoodsQuotaDetails.Where(x => x.GoodsQuotaId == goodsQuotaId).ToListAsync();
        var goodDetailDeletes = await _dbContext.GoodDetails.Where(x => goodIds.Contains(x.GoodID ?? 0)).ToListAsync();
        _dbContext.GoodDetails.RemoveRange(goodDetailDeletes);

        var goods = await _dbContext.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var goodDetails = new List<GoodDetail>();
        foreach (var goodId in goodIds)
        {
            var good = goods.FirstOrDefault(x => x.Id == goodId);
            if (good is null)
            {
                throw new Exception("Cannot find good");
            }

            good.GoodsQuotaId = goodsQuotaId;
            foreach (var goodsQuotaDetail in goodsQuotaDetails)
            {
                goodDetails.Add(new GoodDetail
                {
                    GoodID = goodId,
                    Quantity = goodsQuotaDetail.Quantity,
                    Account = goodsQuotaDetail.Account,
                    AccountName = goodsQuotaDetail.AccountName,
                    Detail1 = goodsQuotaDetail.Detail1,
                    DetailName1 = goodsQuotaDetail.DetailName1,
                    Detail2 = goodsQuotaDetail.Detail2,
                    DetailName2 = goodsQuotaDetail.DetailName2,
                    GoodsQuotaId = goodsQuotaId,
                });
            }

        }
        _dbContext.Goods.UpdateRange(goods);
        await _dbContext.GoodDetails.AddRangeAsync(goodDetails);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _dbContext.GoodsQuotas.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.GOOD_QUOTA)}-{procedureNumber}";
    }
    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _dbContext.GoodsQuotas.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "DM");
    }

}
