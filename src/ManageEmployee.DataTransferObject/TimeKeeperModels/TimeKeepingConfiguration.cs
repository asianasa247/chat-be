namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingConfiguration
{
    public double LatitudeCenter { get; set; }
    public double LongitudeCenter { get; set; }
    public int Radius { get; set; }
    public string PublicIpV4 { get; set; }
    public bool LocationValidationEnable { get; set; }
    public bool IpValidationEnable { get; set; }
}
