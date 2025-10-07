using System.Linq.Expressions;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountService
{
    Task<string> Create(ChartOfAccount entity, int year);

    Task<string> CreateDetail(ChartOfAccount entity, int year);

    Task<string> DeleteDetail(long id, int year);

    Task<List<ChartOfAccountModel>> GetAllAccounts(PagingRequestModel param, int year);

    Task<List<ChartOfAccountModel>> GetAllDetails(int currentPage, int pageSize, string parentCode,
        string warehouseCode, string searchCode, int year, int id = 0, int isInternal = 0);

    Task<int> Count(int year, Expression<Func<ChartOfAccount, bool>> where = null);

    Task<IEnumerable<ChartOfAccountModel>> GetAllByDisplayInsert(PagingRequestModel param, int year);

    Task<List<ChartOfAccountModel>> ExportGetAllArisingAccounts(int year);

    Task<string> ImportFromExcelTaiKhoanArising(List<ChartOfAccount> data, int year);

    Task<List<LookupValue>> GetLookupValues(string scope);

    List<ChartOfAccountModel> ExportGetAllAccounts(int year);

    Task<List<ChartOfAccountModel>> ExportAccountChiTiet1(string code, bool isExportAll, string warehouseCode, int year);

    List<Warehouse> GetListWarehouse();

    Task<string> ImportFromExcel(List<ChartOfAccount> data, int year);

    Task<string> ImportFromExcelCT1(List<ChartOfAccountImportModel> data, string codeParent, int year);

    Task<ObjectReturn> CheckAccountTest(string code, int year);

    Task<ChartOfAccount> GetAccountByCode(string accountCode, int year, string parentRef = "", string wareHouseCode = "");

    Task<List<ChartOfAccountModel>> GetAllAccountCustomer(int year);

    Task<List<ChartAccountDropDownViewModel>> GetAllAccountSelections(List<int> classifications, int year);

    Task<ObjectReturn> CheckAccountDetail(ChartOfAccount data, int year);

    Task<string> CreateAccountGroup(ChartOfAccountGroup accountGroup, int year);

    Task<List<ChartOfAccountGroupModel>> GetAllGroups(int year);

    Task<string> UpdateGroupDetails(ChartOfAccountGroupModel model, int year);

    Task<string> DeleteGroup(string groupId, int year);

    void UpdateAccount(ChartOfAccount account);

    Task<string> UpdateArisingAccount(int year, string dbName);

    Task<string> GetCodeAuto(string parentRef, int isInternal, int year);

    Task TransferAccount(int currentYea, int year);
}
