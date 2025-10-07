using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web;

public class WebCareerServices : IWebCareerService
{
    private readonly ApplicationDbContext _context;

    public WebCareerServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public CareerViewModel Create(CareerViewModel request)
    {
        var entity = new Career();
        entity.Title = request.Title;
        entity.Type = request.Type;
        entity.Group = request.Group;
        entity.Location = request.Location;
        entity.Salary = request.Salary;
        entity.WorkingMethod = request.WorkingMethod;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.Department = request.Department;
        entity.ExpiredApply = request.ExpiredApply;
        entity.Description = request.Description;
        entity.UpdatedAt = DateTime.Now;
        entity.CreatedAt = DateTime.Now;
        entity.ImageUrl = request.ImageUrl;

        _context.Career.Add(entity);
        _context.SaveChanges();
        return new CareerViewModel(entity);
    }

    public void Delete(int id)
    {
        var itemDelete = _context.Career.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefault();
        if (itemDelete != null)
        {
            itemDelete.IsDelete = true;
            itemDelete.DeleteAt = DateTime.Now;
            _context.Career.Update(itemDelete);
            _context.SaveChanges();
        }
    }

    public IEnumerable<CareerViewModel> GetAll()
    {
        var data = _context.Career.Where(x => x.IsDelete != true).Select(x => new CareerViewModel(x)).ToList();
        return data;
    }

    public CareerViewModel GetById(int id)
    {
        try
        {
            var itemData = _context.Career.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefault();
            if (itemData != null)
            {
                return new CareerViewModel(itemData);
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<PagingResult<CareerViewModel>> SearchCareer(CareerPagingRequestModel searchRequest)
    {
        try
        {
            var careers = _context.Career.Where(x => x.IsDelete != true)
                                         .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) ||
                                                     (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(searchRequest.SearchText.ToLower())))
                                         .Where(x => searchRequest.Type == null || x.Type == searchRequest.Type)
                                         .Select(x => new CareerViewModel(x));

            return new PagingResult<CareerViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = await careers.CountAsync(),
                Data = await careers.Skip((searchRequest.Page) * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<CareerViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = 0,
                Data = new List<CareerViewModel>()
            };
        }
    }

    public CareerViewModel Update(CareerViewModel request)
    {
        var itemUpdate = _context.Career.Find(request.Id);
        if (itemUpdate == null)
        {
            return null;
        }
        itemUpdate.Title = request.Title;
        itemUpdate.Group = request.Group;
        itemUpdate.Location = request.Location;
        itemUpdate.Salary = request.Salary;
        itemUpdate.WorkingMethod = request.WorkingMethod;
        itemUpdate.StartTime = request.StartTime;
        itemUpdate.EndTime = request.EndTime;
        itemUpdate.Department = request.Department;
        itemUpdate.ExpiredApply = request.ExpiredApply;
        itemUpdate.Description = request.Description;
        itemUpdate.UpdatedAt = DateTime.Now;
        itemUpdate.Type = request.Type;
        itemUpdate.ImageUrl = request.ImageUrl;

        _context.Career.Update(itemUpdate);
        _context.SaveChanges();
        return new CareerViewModel(itemUpdate);
    }
}