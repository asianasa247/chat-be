using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Generators;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[ApiController]
[Route("api/pdf-generator")]
public class PdfGeneratorController: ControllerBase
{
    private readonly IPdfGeneratorService _generatorService;

    public PdfGeneratorController(IPdfGeneratorService generatorService)
    {
        _generatorService = generatorService;
    }

    [HttpPost("from-html")]
    public async Task<IActionResult> GenerateFromHtml([FromQuery] string type, [FromBody]GeneratePdfOption option)
    {
        var pdfBytes = await _generatorService.GeneratePdf(option.Content, type);
        return File(pdfBytes, "application/pdf", "generated.pdf");
    }
}