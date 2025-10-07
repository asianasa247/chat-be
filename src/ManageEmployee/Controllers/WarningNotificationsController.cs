using ManageEmployee.JobSchedules.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarningNotificationsController : ControllerBase
{
    private readonly IWarningNotificationService _warningNotificationService;
    public WarningNotificationsController(IWarningNotificationService warningNotificationService)
    {
        _warningNotificationService = warningNotificationService;
    }
}
