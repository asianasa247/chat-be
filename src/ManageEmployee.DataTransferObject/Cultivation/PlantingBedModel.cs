namespace ManageEmployee.DataTransferObject.Cultivation
{
    public class PlantingBedModel
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Note { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Quantity { get; set; }
        public int? StartYear { get; set; }
        public int TypeId { get; set; }
        public DateTime? HarvestDate { get; set; }
    }
}
