using ManageEmployee.DataLayer.Service.ViettelPostServices;
using ManageEmployee.DataTransferObject.ViettelPostModels;
using ManageEmployee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Controllers.ViettelPostControllers;

[ApiController]
[Route("api/ViettelPost/location")]
public class ViettelPostController : ControllerBase
{
    private readonly IViettelPostService _viettelPostService;
    private readonly ViettelPostOption _options;
    public ViettelPostController(
        IViettelPostService viettelPostService, 
        IOptions<ViettelPostOption> options)
    {
        _viettelPostService = viettelPostService;
        _options = options.Value;
    }
}