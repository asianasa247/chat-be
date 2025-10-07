namespace ManageEmployee.DataTransferObject.StatusModels;

public class StatusModel
{
    public class MapStatus
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }
        public bool StatusDetect { get; set; }
        public string? Color { get; set; }
    }
}