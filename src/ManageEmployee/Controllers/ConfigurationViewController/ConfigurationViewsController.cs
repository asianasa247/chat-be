using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.ConfigurationViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers.ConfigurationViewController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationViewsController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        public ConfigurationViewsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] DTOConfigurationViews model)
        {
            var configurationView = await dbContext.ConfigurationViews.FirstOrDefaultAsync(cfv => cfv.Id == Id);
            if (configurationView == null)
            {
                return NotFound();
            }
            configurationView.ViewName = model.ViewName;
            configurationView.FieldName = model.FieldName;
            configurationView.Value = model.Value;
            
            try
            {
                dbContext.Update(configurationView);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var configurationview = await dbContext.ConfigurationViews.ToListAsync();
                return Ok(configurationview);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var configurationview = await dbContext.ConfigurationViews.FirstOrDefaultAsync(cfv => cfv.Id == Id);

            if (configurationview == null)
            {
                return NotFound();
            }

            return Ok(configurationview);
        }
    }
}
