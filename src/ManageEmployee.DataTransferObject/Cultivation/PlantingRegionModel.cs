namespace ManageEmployee.DataTransferObject.Cultivation
{
    public class PlantingRegionModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Note { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Manager { get; set; }
        public int? Quantity { get; set; }
        public int TypeId { get; set; }
        public decimal? Area { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public string? Address { get; set; }
        public string? IssuerUnitCode { get; set; }
    }
}
