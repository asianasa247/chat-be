using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Handlers.ChartOfAccount;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;

namespace ManageEmployee.Validators;

public class ChartOfAccountValidator : IChartOfAccountValidator
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ChartOfAccountValidator(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChartOfAccountValidationResult> ValidateCreateNewAccount(ChartOfAccount account, int year)
    {
        var result = new ChartOfAccountValidationResult();
        result.Handlers.Add(new AccountCrudActionPermissionHandler());
        if (string.IsNullOrEmpty(account.Code) || account.Code.Length < 3 || account.Code.Length > 6)
            return new ChartOfAccountValidationResult(ErrorMessages.AccountCodeMustBetween3And6);
        if (await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == account.Code && x.Id != account.Id))
            return new ChartOfAccountValidationResult(ErrorMessages.AccountCodeAlreadyExist);

        if (account.Code.Length == 3)
        {
            return result;
        }

        if (account.Code.Length > 3)
        {
            result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, true));

            //Kiểm tra có tồn tại tài khoản cha hay không
            if (!await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == account.ParentRef))
                return new ChartOfAccountValidationResult(ErrorMessages.MissingParentAccount);

            //Kiểm tra tài khoản cha có nằm trong nhóm đồng bộ hay không
            //if (await _context.ChartOfAccounts.AnyAsync(x =>
            //    x.Code == account.ParentRef && x.ChartOfAccountGroupLinks.Any(y => y.AccountCode == x.Code)))
            //    return new ChartOfAccountValidationResult(ErrorMessages.ParentAccountIsInSyncGroupAndCanNotCreate);

            //Kiểm tra tài khoản cha có chi tiết hay không
            if (await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == account.ParentRef && x.HasDetails))
            {
                result.Handlers.Add(new TransferDetailFromParentToChildAccountHandler(_context));
                return result;
            }

            //Kiểm tra có tài khoản cùng cấp hay không
            if (await _context.GetChartOfAccount(year).AnyAsync(
                x => x.Type == account.Type && x.Code.StartsWith(account.ParentRef)))
                result.Handlers.Add(new ExistingSameLevelAccountCrudActionPermissionHandler(_context));
            else
                result.Handlers.Add(new NotExistingSameLevelAccountCrudActionPermissionHandler(_context));
            return result;
        }

        return result;
    }

    public async Task<ChartOfAccountValidationResult> ValidateDeleteAccount(ChartOfAccount account, int year)
    {
        var result = new ChartOfAccountValidationResult();

        //Kiểm tra có tài khoản con hay không
        if (await _context.GetChartOfAccount(year).AnyAsync(x => x.ParentRef == account.Code && x.Type <= 4))
        {
            return new ChartOfAccountValidationResult(ErrorMessages.FailedToDelete);
        }

        if (account.Code?.Length > 3)
        {
            //Kiểm tra có chi tiết hay không
            if (account.HasDetails)
            {
                //Kiểm tra có tài khoản cùng cấp hay không
                if (await _context.GetChartOfAccount(year).AnyAsync(x =>
                    x.Type == account.Type && x.ParentRef == account.ParentRef && x.Id != account.Id))
                    return new ChartOfAccountValidationResult(ErrorMessages.FailedToDelete);

                result.Handlers.Add(new TransferDetailToOneUpperLevelAccountHandler(_context));
                result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, true));
                return result;
            }

            //Kiểm tra có tài khoản cùng cấp hay không
            if (!await _context.GetChartOfAccount(year).AnyAsync(x =>
                x.Type == account.Type && x.ParentRef == account.ParentRef && x.Id != account.Id))
            {
                result.Handlers.Add(new ParentCrudActionPermissionHandler(_context));
                result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, true));
            }

            return result;
        }

        //Kiểm tra tài khoản gốc có chi tiết hay không
        if (account.HasDetails)
            return new ChartOfAccountValidationResult(ErrorMessages.FailedToDelete);

        return result;
    }

    //Kiểm tra có chi tiết và có tài khoản con đã được validate trên client bằng việc disabled textbox code.
    public async Task<ChartOfAccountValidationResult> ValidateUpdateAccount(ChartOfAccountModel model,
        ChartOfAccount oldAccount, ChartOfAccount toMergeAccount, int year)
    {
        var result = new ChartOfAccountValidationResult();

        if (oldAccount.Code.Length < 3 || oldAccount.Code.Length > 6)
            return new ChartOfAccountValidationResult(ErrorMessages.AccountCodeMustBetween3And6);

        //Kiểm tra code có bị trùng không
        if (!await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == model.Code && x.Id != model.Id))
        {
            //Kiểm tra có tồn tại tài khoản cha hay không
            if (model.Code.Length > 3)
            {
                var parentRef = model.Code.Substring(0, model.Code.Length - 1);
                if (!await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == parentRef && x.Id != model.Id))
                    return new ChartOfAccountValidationResult(ErrorMessages.MissingParentAccount);
                return result;
            }
        }

        //Kiểm tra code có được đổi hay không
        if (model.Code != oldAccount.Code && toMergeAccount != null)
        {
            //Kiểm tra account đang tồn tại có cùng loại với account định gộp hay không

            if (toMergeAccount.AccGroup != oldAccount.AccGroup)
                return new ChartOfAccountValidationResult(ErrorMessages.AccGroupDoesNotMatchForMerging);

            //Kiểm tra account đang tồn tại có cùng cấp với account định gộp hay không
            if (toMergeAccount.Type != oldAccount.Type)
                return new ChartOfAccountValidationResult(ErrorMessages.AccGroupDoesNotMatchForMerging);

            //Kiểm tra cả 2 account có chi tiết hay không
            if (oldAccount.HasDetails || toMergeAccount.HasDetails)
                return new ChartOfAccountValidationResult(ErrorMessages
                    .AccountContainsDetailsAndDoesNotAllowToBeMerged);

            if (await _context.GetChartOfAccountGroupLink(year).AnyAsync(x =>
                x.CodeChartOfAccount == toMergeAccount.Code || x.CodeChartOfAccount == oldAccount.Code))
                return new ChartOfAccountValidationResult(ErrorMessages.AccountIsInGroupSyncAndDoesNotAllowToEdit);
        }

        return result;
    }

    public async Task<ChartOfAccountValidationResult> ValidateCreateAccountDetail(ChartOfAccount account, int year)
    {
        var result = new ChartOfAccountValidationResult();
        result.Handlers.Add(new AccountCrudActionPermissionHandler());
        result.Handlers.Add(new ParentCrudActionPermissionHandler(_context, true, true, false));
        result.Handlers.Add(new CloneDetailWithAccGroupIs3Handler(_context, _mapper));
        result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, true));
        result.Handlers.Add(
            new SyncAccountDetailBySyncGroupHandler(_context, null, _mapper, AccountSyncAction.Create));

        //Kiểm tra độ dài mã
        if (string.IsNullOrEmpty(account.Code) || account.Code.Length > 60)
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeMustBeLessThan60);

        //Kiểm tra mã chi tiết có bị trùng hay không
        if (await _context.GetChartOfAccount(year).AnyAsync(x =>
            x.Code == account.Code && x.Id != account.Id && x.ParentRef == account.ParentRef &&
            x.Type == account.Type && x.WarehouseCode == account.WarehouseCode))
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeAlreadyExist);

        //Kiểm tra mã chi tiết có trùng với chi tiết cha hay không
        if (account.Type == 6 && account.Code == account.ParentRef.Split(':').Last())
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeDuplicateWithParentCode);

        return result;
    }

    public async Task<ChartOfAccountValidationResult> ValidateUpdateAccountDetail(ChartOfAccountModel model,
        ChartOfAccount account, ChartOfAccount toMergeAccount, int year)
    {
        var result = new ChartOfAccountValidationResult();
        result.Handlers.Add(
            new SyncAccountDetailBySyncGroupHandler(_context, model, _mapper, AccountSyncAction.Edit));

        //Kiểm tra độ dài mã
        if (string.IsNullOrEmpty(account.Code) || account.Code.Length > 60)
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeMustBeLessThan60);

        //Kiểm tra mã chi tiết có trùng hay không
        if (await _context.GetChartOfAccount(year).AnyAsync(x =>
            x.Code == account.Code && x.Id != account.Id && x.ParentRef == account.ParentRef &&
            x.WarehouseCode == account.WarehouseCode))
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeAlreadyExist);

        //Kiểm tra mã chi tiết có trùng với chi tiết cha hay không
        if (account.Type == 6 && account.Code == account.ParentRef.Split(':').Last())
            return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailCodeDuplicateWithParentCode);

        if (toMergeAccount != null && toMergeAccount.Type == 5)
        {
            //Kiểm tra mã định gộp có chi tiết con hay không
            if (await _context.GetChartOfAccount(year).AnyAsync(x =>
                x.ParentRef == $"{toMergeAccount.Code}:{toMergeAccount.ParentRef}"))
                return new ChartOfAccountValidationResult(ErrorMessages.AccountDetailContainsChild);
        }

        return result;
    }

    public async Task<ChartOfAccountValidationResult> ValidateDeleteAccountDetail(ChartOfAccount account, int year)
    {
        var result = new ChartOfAccountValidationResult();
        result.Handlers.Add(new RemoveDeleteWithAccGroup3Handler(_context));
        result.Handlers.Add(
            new SyncAccountDetailBySyncGroupHandler(_context, null, _mapper, AccountSyncAction.Delete));
        //Kiểm tra có còn chi tiết cùng cấp không
        if (account.Type == 6)
        {
            if (!await _context.GetChartOfAccount(year).AnyAsync(x =>
                x.ParentRef == account.ParentRef && x.Type == account.Type && x.Id != account.Id &&
                x.WarehouseCode == account.WarehouseCode))
            {
                result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, false));
                result.Handlers.Add(new ParentCrudActionPermissionHandler(_context));
            }
        }
        else
        {
            if (!await _context.GetChartOfAccount(year).AnyAsync(x =>
                x.ParentRef == account.ParentRef && x.Type == account.Type && x.Id != account.Id))
            {
                result.Handlers.Add(new ParentCrudActionPermissionHandler(_context));
                result.Handlers.Add(new DefineParentHasChildVariableHandler(_context, false));
            }
        }

        return await Task.FromResult(result);
    }
}