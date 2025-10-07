using ManageEmployee.Handlers.ChartOfAccount;

namespace ManageEmployee.Validators;

public class ChartOfAccountValidationResult
{
    public string? ErrorMessage { get; set; }

    public List<IChartOfAccountHandler> Handlers { get; }

    public ChartOfAccountValidationResult(string? errorMessage = null)
    {
        ErrorMessage = errorMessage;
        Handlers = new List<IChartOfAccountHandler>();
    }
}
