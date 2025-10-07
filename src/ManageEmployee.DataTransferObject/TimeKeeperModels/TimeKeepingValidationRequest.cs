using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingValidationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string DeviceId { get; set; }
    public DateTime TimeFrameFrom { get; set; }
    public DateTime TimeFrameTo { get; set; }
    public IFormFile File { get; set; }
}
