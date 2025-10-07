namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingValidationResult
{
    public Location ClientLocation { get; set; }
    public Location ValidationLocation { get; set; }
    public string ClientIp { get; set; }
    public string ValidationIp { get; set; }
    public double Distance { get; set; }
    public bool IsValid => IpValidation || LocationValidation;
    public bool IpValidation { get; set; }
    public bool LocationValidation { get; set; }
    public string DeviceId { get; set; }
    public int TargetId { get; set; } // Checkin/out position
}