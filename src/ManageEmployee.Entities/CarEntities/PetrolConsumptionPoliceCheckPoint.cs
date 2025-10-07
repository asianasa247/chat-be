namespace ManageEmployee.Entities.CarEntities;

public class PetrolConsumptionPoliceCheckPoint
{
    public int Id { get; set; }
    public int PetrolConsumptionId { get; set; }
    public string? PoliceCheckPointName { get; set; }
    public double Amount { get; set; }
    public bool IsArise { get; set; }

}
