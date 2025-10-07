using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Validators;

public interface IChartOfAccountValidator
{
    Task<ChartOfAccountValidationResult> ValidateCreateNewAccount(ChartOfAccount account, int year);
    Task<ChartOfAccountValidationResult> ValidateDeleteAccount(ChartOfAccount account, int year);

    Task<ChartOfAccountValidationResult>
        ValidateUpdateAccount(ChartOfAccountModel model, ChartOfAccount oldAccount, ChartOfAccount toMergeAccount, int year);

    Task<ChartOfAccountValidationResult> ValidateCreateAccountDetail(ChartOfAccount account, int year);

    Task<ChartOfAccountValidationResult> ValidateUpdateAccountDetail(ChartOfAccountModel model,
        ChartOfAccount account, ChartOfAccount toMergeAccount, int year);

    Task<ChartOfAccountValidationResult> ValidateDeleteAccountDetail(ChartOfAccount account, int year);
}
