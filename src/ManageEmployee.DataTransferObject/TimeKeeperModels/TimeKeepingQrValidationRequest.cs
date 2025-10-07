using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingQrValidationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string DeviceId { get; set; }
    public DateTime TimeFrameFrom { get; set; }
    public DateTime TimeFrameTo { get; set; }
    public int TagertId { get; set; }
}
