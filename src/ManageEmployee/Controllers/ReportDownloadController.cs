using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportDownloadController : Controller
{
    private readonly IFileService _fileService;

    public ReportDownloadController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    [Route("DownloadReportFromFile")]
    public IActionResult DownloadFileDoc(string? filename, string? fileType)
    {
        try
        {
            return DownLoadFileExport(filename, @"ExportHistory\\"+fileType.ToUpper());
        }
        catch
        {
            return BadRequest();
        }
    }

    
    [HttpPost("uploadImage")]
    public IActionResult uploadImage([FromForm] IFormFile file)
    {
        try
        {
            var fileName = string.Empty;
            string fileType = System.IO.Path.GetExtension(file.FileName);
            string FileId = Guid.NewGuid().ToString();

            fileName = _fileService.Upload(file, "Images", FileId + fileType);
            return Ok(new { imageUrl = fileName });
            //return BadRequest(new { msg = file.FileName });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpPost("deleteImages")]
    public IActionResult deleteImages([FromBody] List<DeleteImageModel> requests)
    {
        try
        {
            for (int i = 0; i < requests.Count; i++)
            {
                _fileService.DeleteFileUpload(requests[i].imageUrl);
            }
            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("upload-contract")]
    public IActionResult UpLoadContrac([FromForm] IFormFile file)
    {
        try
        {
            var fileName = _fileService.Upload(file, "Contract", file.FileName);
            return Ok(new { imageUrl = fileName });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet]
    [Route("download-contract")]
    public IActionResult DownloadContractSample(string? type = "probationary")
    {
        try
        {
            if(type == "labor")
            {
                return DownLoadFileExport("HopDongLaoDong.docx", @"Uploads\\Contract\\");
            }
            else
            {
                return DownLoadFileExport("HopDongThuViec.docx", @"Uploads\\Contract\\");
            }
        }
        catch
        {
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("download-contract-type")]
    public IActionResult DownloadContractType(string? linkFile )
    {
        try
        {
            string filename = "";
            if (linkFile.Contains("\\\\"))
            {
                filename = linkFile.Split("\\\\")[2];
            }
            else
            {
                filename = linkFile.Split("\\")[2];
            }
            string pathRoot = linkFile.Replace(filename, "");
            return DownLoadFileExport(filename, pathRoot);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("download-file-template")]
    public IActionResult DownloadFileTemplate(string? fileName)
    {
        return DownLoadFileExport(fileName, @"Uploads\\\Excel\\");
    }

    FileResult DownLoadFileExport(string? pFileName, string? pathRoot)
    {
        string filename = pFileName;
        string filepath = Path.Combine(Directory.GetCurrentDirectory(), pathRoot, pFileName);
        byte[] filedata = System.IO.File.ReadAllBytes(filepath);

        string contentType;
        new FileExtensionContentTypeProvider().TryGetContentType(filepath, out contentType);
        contentType = contentType ?? "application/octet-stream";
        var cd = new System.Net.Mime.ContentDisposition
        {
            FileName = filename,
            Inline = true,
        };
        Response.Headers.Add("Content-Disposition", cd.ToString());
        return File(filedata, contentType);
    }
}
