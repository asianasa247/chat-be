namespace ManageEmployee.DataTransferObject.Reports;

public class SaveMovedModelBase
{
    public string? title { get; set; }
    public string? code { get; set; }
    public string? subs { get; set; }
    public double this_year { get; set; }
    public double prev_year { get; set; }
    public double accumulated_start_year { get; set; }
    public string? level { get; set; }
    public string? debit_code { get; set; }
    public string? credit_code { get; set; }
    public List<string> credit_code_lst
    {
        get
        {
            return !string.IsNullOrEmpty(credit_code) ? credit_code.Split(",").ToList() : null;
        }
    }
    public List<string> first_debit_lst
    {
        get
        {
            return !string.IsNullOrEmpty(first) ? first.Split("|").ToList() : null;
        }
    }
    public List<string> second_credit_lst
    {
        get
        {
            return !string.IsNullOrEmpty(second) ? second.Split("|").ToList() : null;
        }
    }
    public string? first { get; set; }
    public string? second { get; set; }
    public string? rowtype { get; set; }
    public string? note { get; set; }
    public List<SaveMovedModelBase>? items { get; set; }
}
