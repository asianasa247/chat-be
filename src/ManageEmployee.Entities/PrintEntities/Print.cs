namespace ManageEmployee.Entities.PrintEntities;
public class Print
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Size { get; set; }
    public double? MarginLeft { get; set; }
    public double? MarginRight { get; set; }
    public double? MarginTop { get; set; }
    public double? MarginBottom { get; set; }
}