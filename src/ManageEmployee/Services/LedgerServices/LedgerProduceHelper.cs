using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.P_Procedures;

namespace ManageEmployee.Services.LedgerServices;

public class LedgerProduceHelper : ILedgerProduceHelper
{
    private readonly IProcedureExportHelper _procedureExportHelper;

    public LedgerProduceHelper(IProcedureExportHelper procedureExportHelper)
    {
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<string> SignPlace(string orginalCompanyName, int id, string procedureCode)
    {
        var userPosition = "Người giao hàng";
        var _signtxtItem = @"<td class='td-no-border'>
                                        <div class='signature-item'>
                                            <p class='signature-item-signer'>{{{UserPosition}}}</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <p class='signature-item-signer'>{{{UserName}}}</p><br/>
                                        </div>
                                    </td>";

        _signtxtItem = _signtxtItem
               .Replace("{{{UserPosition}}}", userPosition)
               .Replace("{{{UserName}}}", orginalCompanyName?.ToUpper());

        var signProcedure = await _procedureExportHelper.SignPlaceSameTr(id, procedureCode);
        return _signtxtItem + signProcedure;
    }
}