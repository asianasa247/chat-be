namespace ManageEmployee.DataTransferObject.CompanyModels;

public class JobModel
{
    public class MapJob
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }
        public bool Status { get; set; }
        public string? Color { get; set; }
    }
}