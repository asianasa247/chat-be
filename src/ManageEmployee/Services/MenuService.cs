using AutoMapper;
using Common.Constants;
using Common.Extensions;
using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageEmployee.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserRoleService _userRoleService;
    public MenuService(ApplicationDbContext context, IMapper mapper, IUserRoleService userRoleService)
    {
        _context = context;
        _mapper = mapper;
        _userRoleService = userRoleService;
    }

    public async Task<IEnumerable<MenuViewModel>> GetAll(bool isParent)
    {
        var data = await _context.Menus.WhereIf(isParent, x => x.IsParent).Select(x => new MenuViewModel
        {
            Id = x.Id,
            Name = x.Name,
            NameEN = x.NameEN,
            NameKO = x.NameKO,
            Code = x.Code,
            CodeParent = x.CodeParent,
            IsParent = x.IsParent,
            Order = x.Order
        }).ToListAsync();
        return data;
    }

    public async Task<PagingResult<MenuViewPagingModel>> GetAll(int pageIndex, int pageSize, string keyword,
        bool isParent, string codeParent, List<string> listRole, int userId, int? userRoleId, int? type = null)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 0;

            if (!keyword.IsNullOrEmpty())
            {
                keyword = keyword.ToLower();
            }
            var result = new PagingResult<MenuViewPagingModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
            };
            List<int> menuIds = new List<int>();
            if (!listRole.Contains(UserRoleConst.SuperAdmin))
            {
                if (listRole.Contains(UserRoleConst.AdminBranch))
                {
                    int _userRoleId = await _context.UserRoles.Where(x => x.Code == UserRoleConst.AdminBranch).Select(x => x.Id).FirstOrDefaultAsync();
                    menuIds = await _context.MenuRoles.Where(x => x.UserRoleId == _userRoleId).Select(x => x.MenuId ?? 0).Distinct().ToListAsync();
                }
                else
                {
                    var userRoleIds = await _context.UserRoles.Where(x => listRole.Contains(x.Code)).Select(x => x.Id).ToListAsync();
                    menuIds = await _context.MenuRoles.Where(x => userRoleIds.Contains(x.UserRoleId ?? 0)).Select(x => x.MenuId ?? 0).Distinct().ToListAsync();
                }
            }
            List<int> menuIdSlects = new List<int>();
            if (userRoleId.HasValue && userRoleId > 0)
            {
                menuIdSlects = await _context.MenuRoles.Where(x => x.UserRoleId == userRoleId).Select(x => x.MenuId ?? 0).ToListAsync();
            }

            var menus = _context.Menus
                            .WhereIf(!keyword.IsNullOrEmpty(), x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword))
                            .WhereIf(!codeParent.IsNullOrEmpty(), x => x.CodeParent == codeParent)
                            .WhereIf(isParent, x => x.IsParent)
                            .WhereIf(userRoleId.HasValue && userRoleId > 0, x => menuIdSlects.Contains(x.Id))
                            .Where(x => listRole.Contains(UserRoleConst.SuperAdmin) || menuIds.Contains(x.Id))
                            .Select(x => new MenuViewPagingModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                NameEN = x.NameEN,
                                NameKO = x.NameKO,
                                Code = x.Code,
                                CodeParent = x.CodeParent,
                                IsParent = x.IsParent,
                                Order = x.Order
                            });
            result.TotalItems = await menus.CountAsync();
            result.Data = await menus.Skip((pageIndex) * pageSize).Take(pageSize).ToListAsync();

            foreach (var data in result.Data)
            {
                var userRoleIds = await _context.MenuRoles.Where(x => x.MenuId == data.Id).Select(x => x.UserRoleId).ToListAsync();
                data.Roles = string.Join(",", await _context.UserRoles.Where(x => userRoleIds.Contains(x.Id)).Select(x => x.Title).ToListAsync());
            }
            return result;
        }
        catch
        {
            return new PagingResult<MenuViewPagingModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<MenuViewPagingModel>()
            };
        }
    }

    public async Task Create(MenuViewModel request)
    {
        await _context.Database.BeginTransactionAsync();
        var existCode = _context.Menus.Where(
            x => x.Code.ToLower() == request.Code.ToLower()).FirstOrDefault();
        if (existCode != null)
        {
            throw new ErrorException(ErrorMessages.MenuCodeAlreadyExist);
        }
        Menu menu = _mapper.Map<Menu>(request);
        _context.Menus.Add(menu);
        _context.SaveChanges();
        if (request.listItem != null)
        {
            foreach (var item in request.listItem)
            {
                MenuRole menuRole = _mapper.Map<MenuRole>(item);
                menuRole.MenuId = menu.Id;
                menuRole.MenuCode = menu.Code;
                _context.MenuRoles.Add(menuRole);
            }
        }
        await _context.SaveChangesAsync();
        _context.Database.CommitTransaction();

    }

    public async Task<MenuViewModel> GetById(int id, List<string> listRole, int userId)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var menuRoleQuery = _context.MenuRoles.Where(x => x.MenuId == id);
        var role = await _userRoleService.GetAll(userId, listRole);
        var userRoleIds = role.Select(x => x.Id);
        if (listRole.Contains(UserRoleConst.AdminBranch))
        {
            menuRoleQuery = menuRoleQuery.Where(x => userRoleIds.Contains(x.UserRoleId ?? 0));
        }
        else if (listRole.Contains(UserRoleConst.SuperAdmin))
        {
            menuRoleQuery = menuRoleQuery.Where(x => x.Id > 0);
        }
        else
        {
            menuRoleQuery = menuRoleQuery.Where(x => userRoleIds.Contains(x.UserRoleId ?? 0));
        }

        var menuRoles = await menuRoleQuery.Select(x => _mapper.Map<MenuRoleViewModel>(x)).ToListAsync();

        var menuView = _mapper.Map<MenuViewModel>(menu);
        menuView.listItem = menuRoles;
        return menuView;
    }

    public async Task Update(MenuViewModel request, List<string> listRole, int userId)
    {
        await _context.Database.BeginTransactionAsync();
        var menu = await _context.Menus.FindAsync(request.Id);
        if (menu == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var checkMenuCode = await _context.Menus.Where(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != menu.Id).FirstOrDefaultAsync();
        if (checkMenuCode != null && checkMenuCode.Id != menu.Id)
        {
            throw new ErrorException(ErrorMessages.MenuCodeAlreadyExist);
        }
        menu.Name = request.Name;
        menu.NameEN = request.NameEN;
        menu.NameKO = request.NameKO;
        menu.Code = request.Code;
        menu.CodeParent = request.CodeParent;
        menu.IsParent = request.IsParent;
        menu.Order = request.Order;

        _context.Menus.Update(menu);

        List<int> useRoleIds = new List<int>();
        if (listRole.Contains(UserRoleConst.AdminBranch))
        {
            useRoleIds = await _context.UserRoles.Where(x => x.UserCreated == userId || listRole.Contains(x.Code)).Select(x => x.Id).ToListAsync();
        }
        else if (listRole.Contains(UserRoleConst.SuperAdmin))
        {
            useRoleIds = await _context.UserRoles.Select(x => x.Id).ToListAsync();
        }
        else
        {
            useRoleIds = await _context.UserRoles.Where(x => listRole.Contains(x.Code)).Select(x => x.Id).ToListAsync();
        }
        var menuRoleDel = await _context.MenuRoles.Where(x => x.MenuId == request.Id
            && useRoleIds.Contains(x.UserRoleId ?? 0)).ToListAsync();

        _context.MenuRoles.RemoveRange(menuRoleDel);
        if (request.listItem != null)
        {
            foreach (var item in request.listItem)
            {
                if (!useRoleIds.Contains(item.UserRoleId ?? 0))
                {
                    throw new ErrorException("Bạn không có quyền update role!");
                }
                MenuRole menuRole = _mapper.Map<MenuRole>(item);
                menuRole.Id = 0;
                menuRole.MenuId = menu.Id;
                menuRole.MenuCode = menu.Code;
                await _context.MenuRoles.AddAsync(menuRole);
            }
        }

        await _context.SaveChangesAsync();
        await _context.Database.CommitTransactionAsync();
    }

    public async Task Delete(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        var menuRoles = await _context.MenuRoles.Where(x => x.MenuId == id).ToListAsync();
        if (menu != null)
        {
            var menuChild = await _context.Menus.Where(x => x.CodeParent == menu.Code).ToListAsync();
            if (menuChild.Count > 0)
            {
                foreach (var item in menuChild)
                {
                    item.CodeParent = null;
                    _context.Menus.Update(item);
                }
            }
            _context.Menus.Remove(menu);
            _context.MenuRoles.RemoveRange(menuRoles);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<MenuCheckRole> CheckRole(string MenuCode, List<string> roleCodes)
    {
        try
        {
            Menu menu = await _context.Menus.FirstOrDefaultAsync(x => x.Code == MenuCode);
            var useRoleIds = await _context.UserRoles.Where(x => roleCodes.Contains(x.Code)).Select(x => x.Id).ToListAsync();
            MenuCheckRole itemOut = new MenuCheckRole();
            var menuRoles = await _context.MenuRoles.Where(x => x.MenuId == menu.Id && useRoleIds.Contains(x.UserRoleId ?? 0)).ToListAsync();
            if (menuRoles.Count > 0)
            {
                itemOut.Add = menuRoles.Find(x => x.Add)?.Add ?? false;
                itemOut.Edit = menuRoles.Find(x => x.Edit)?.Edit ?? false;
                itemOut.Delete = menuRoles.Find(x => x.Delete)?.Delete ?? false;
                itemOut.View = menuRoles.Find(x => x.View)?.View ?? false;
            }
            itemOut.MenuCode = menu.Code;
            itemOut.Name = menu.Name;
            itemOut.NameEN = menu.NameEN;
            itemOut.NameKO = menu.NameKO;
            return itemOut;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<MenuCheckRole>> GetListMenu(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        List<string> listRole = user.UserRoleIds.Split(",").ToList();
        var menus = await _context.Menus.ToListAsync();
        var userRoles = await _userRoleService.GetAll_Login();
        var roles = userRoles.Where(o => listRole.Contains(o.Id.ToString())).Select(x => x.Code).ToList();

        List<MenuCheckRole> listMenuCheckRole = new List<MenuCheckRole>();

        if (roles.Contains(UserRoleConst.SuperAdmin))
        {
            foreach (var menuCode in menus)
            {
                MenuCheckRole menu = new MenuCheckRole();
                menu.MenuCode = menuCode.Code;
                menu.Add = true;
                menu.Edit = true;
                menu.Delete = true;
                menu.View = true;
                menu.Name = menuCode.Name;
                menu.NameEN = menuCode.NameEN;
                menu.NameKO = menuCode.NameKO;
                menu.Order = menuCode.Order;
                listMenuCheckRole.Add(menu);
            }
        }
        else
        {
            var listMenuRole = await _context.MenuRoles.Where(x => listRole.Contains((x.UserRoleId ?? 0).ToString())).ToListAsync();
            var listMenuCode = listMenuRole.Select(x => x.MenuCode).Distinct().ToList();
            foreach (var menuCode in listMenuCode)
            {
                var menuRole = listMenuRole.Find(x => x.MenuCode == menuCode);
                var menuFind = menus.Find(x => x.Code == menuCode);
                MenuCheckRole menu = new MenuCheckRole();
                menu.MenuCode = menuCode;
                menu.Add = menuRole?.Add ?? false;
                menu.Edit = menuRole?.Edit ?? false;
                menu.Delete = menuRole?.Delete ?? false;
                menu.View = menuRole?.View ?? false;
                menu.Name = menuFind?.Name;
                menu.NameEN = menuFind?.NameEN;
                menu.NameKO = menuFind?.NameKO;
                menu.Order = menuFind?.Order ?? 0;

                listMenuCheckRole.Add(menu);
            }
        }
        return listMenuCheckRole;
    }
}