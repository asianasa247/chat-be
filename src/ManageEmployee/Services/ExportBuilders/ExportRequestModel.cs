namespace ManageEmployee.Services.ExportBuilders;

public record class ExportRequestModel<T>
{
    // Excluding company's fields
    public List<ExportFieldModel> Fields { get; set; }

    public string[] TemplatePath { get; set; }

    public string PrefixFile { get; set; }
    public int ProcedureId { get; set; }
    public string ProcedureCode { get; set; }

    public List<T> Data { get; set; }

    /// <summary>
    /// Para 1: item of Data
    /// Para 2: soThapPhan
    /// Para 3: index of data
    /// </summary>
    public Func<T, int, string, string> GenerateExportRow { get; set; }
}
