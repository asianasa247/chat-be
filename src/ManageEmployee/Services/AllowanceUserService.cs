using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.AllowanceEntities;
using ManageEmployee.Services.Interfaces.Allowances;

namespace ManageEmployee.Services;
public class AllowanceUserService: IAllowanceUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AllowanceUserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public IEnumerable<AllowanceUserViewModel> GetAll(int currentPage, int pageSize, string keyword)
    {

        var items = _context.Users
            .Where(x => !x.IsDelete)
            .Where(x => string.IsNullOrEmpty(keyword) || x.FullName.ToLower().Contains(keyword.ToLower()))
            .Skip(pageSize * currentPage)
            .Take(pageSize);
        
        foreach(var item in items)
        {
            AllowanceUserViewModel itemOut = new();
            itemOut.UserId = item.Id;
            itemOut.UserName = item.FullName;
            itemOut.listItem = _context.AllowanceUsers.Where(x => x.UserId == item.Id).Select(x => _mapper.Map<AllowanceUserDetailViewModel>(x)).ToList();
            yield return itemOut;
        }

    }
    public string Update(AllowanceUserViewModel user)
    {
        var allowasceUserDels = _context.AllowanceUsers.Where(x => x.UserId == user.UserId).ToList();
        _context.AllowanceUsers.RemoveRange(allowasceUserDels);
        var listItem = user.listItem;
        if(listItem != null)
        {
            var allowasceUsers = _mapper.Map<List<AllowanceUser>>(listItem).Select(x => { x.UserId = user.UserId; x.Id = 0; return x; }).ToList();
            _context.AllowanceUsers.AddRange(allowasceUsers);
        }
        _context.SaveChanges();
        return "";
    }
}
