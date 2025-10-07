using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.ContentMailModels;
using ManageEmployee.DataTransferObject.PrizeModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.AreaEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Controllers.ContentMailControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentMailController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        public ContentMailController(ApplicationDbContext dbContext)
        {
                this.dbContext = dbContext;
        }


        [HttpDelete]
        [Route("{Id:int:required}")]
        public async Task<IActionResult> Delete([Required][FromRoute] int? Id)
        {
            var contentmail = await dbContext.ContentMails.FindAsync(Id);
            if (contentmail != null)
            {
                dbContext.Remove(contentmail);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }

        [HttpPut]
        [Route("{Id:int:required}")]
        public async Task<IActionResult> Update([Required][FromRoute] int? Id, [Required][FromBody] ContentMailDTO model)
        {
            var contentMail = await dbContext.ContentMails.FindAsync(Id);
            if (contentMail == null)
            {
                return NotFound();
            }
            contentMail.Title = model.Title;
            contentMail.BodyMail = model.BodyMail;
            contentMail.Type = model.Type;
            dbContext.Update(contentMail);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ContentMailDTO model)
        {
            ContentMail contentMail = new ContentMail();
            contentMail.Title = model.Title;
            contentMail.BodyMail = model.BodyMail;
            contentMail.Type = model.Type;

            try
            {
                await dbContext.AddAsync(contentMail);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var contentMails = await dbContext.ContentMails.ToListAsync();
            return Ok(contentMails);
        }

        // GET: Lấy ContentMail theo Id
        [HttpGet("{Id:int:required}")]
        public async Task<IActionResult> GetById([Required][FromRoute] int? Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }

            var contentMail = await dbContext.ContentMails.FindAsync(Id);
            if (contentMail == null)
            {
                return NotFound();
            }

            return Ok(contentMail);
        }


        [HttpGet("ContentMailPage")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }

            var totalRecords = await dbContext.ContentMails.CountAsync();
            var contentMails = await dbContext.ContentMails
                                              .Skip((pageNumber - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = contentMails
            };

            return Ok(response);
        }


    }
}
