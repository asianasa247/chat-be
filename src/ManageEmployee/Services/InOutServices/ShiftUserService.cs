using System.Linq.Expressions;
using AutoMapper;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.InOutServices;

public class ShiftUserService : IShiftUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ShiftUserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private async Task<ShiftUserModel> GetDataInit(int month, int year)
    {
        var shiftuserCheck = await _context.ShiftUsers.AnyAsync(x => x.Month == month && x.Year == year);
        if (shiftuserCheck)
        {
            throw new ErrorException("Existed record");
        }

        var users = await _context.Users.Where(x => !x.IsDelete && !x.Quit).ToListAsync();
        var listOut = await GetUser(users, month, year);

        return new ShiftUserModel
        {
            Month = month,
            Year = year,
            Items = listOut,
            Name = "Danh sách ca tháng " + month,
        };
    }

    public async Task SetData(ShiftUserModel form, int year)
    {
        // validate
        if(form.Items != null)
        {
            var userDuplicates = form.Items.Where(x => !x.IsDeleted).GroupBy(x => x.UserId).Where(g => g.Count() > 1)
                .Select(y => new
                {
                    userId = y.Key,
                    userName = y.Select(x => x.UserFullName).ToList(),
                }).ToList();

            if (userDuplicates.Any())
            {
                var userNameDuplicates = string.Join(", ", userDuplicates.Select(X => X.userName));
                throw new ErrorException($"Duplicate user with name {userNameDuplicates}");
            }
        }
        var shiftuserCheck = await _context.ShiftUsers.AnyAsync(x => x.Month == form.Month && x.Year == year && x.Id != form.Id);
        if (shiftuserCheck)
        {
            throw new ErrorException("Existed record");
        }

        var shiftUser = _mapper.Map<ShiftUser>(form);
        shiftUser.Year = year;
        _context.ShiftUsers.Update(shiftUser);
        await _context.SaveChangesAsync();

        var dataRemoves = form.Items.Where(x => x.IsDeleted && x.Id > 0).Select(x => _mapper.Map<ShiftUserDetail>(x)).ToList();
        _context.ShiftUserDetails.RemoveRange(dataRemoves);

        var dataUpdates = form.Items.Where(x => !x.IsDeleted && x.Id > 0).Select(x => _mapper.Map<ShiftUserDetail>(x)).ToList();
        dataUpdates = dataUpdates.ConvertAll(x =>
        {
            x.ShiftUserId = shiftUser.Id;
            return x;
        });
        _context.ShiftUserDetails.UpdateRange(dataUpdates);

        var dataAdds = form.Items.Where(x => !x.IsDeleted && x.Id == 0).Select(x => _mapper.Map<ShiftUserDetail>(x)).ToList();
        dataAdds = dataAdds.ConvertAll(x =>
        {
            x.ShiftUserId = shiftUser.Id;
            return x;
        });
        await _context.ShiftUserDetails.AddRangeAsync(dataAdds);

        await _context.SaveChangesAsync();
    }

    public async Task<ShiftUserModel> GetDetail(int userId, int month, int year)
    {
        var user = _context.Users.Where(x => x.Id == userId)
            .FirstOrDefault();

        var shiftUser = await _context.ShiftUsers.FirstOrDefaultAsync(x => x.Month == month && x.Year == year);

        if(shiftUser is null)
        {
            return await GetDataInit(month, year);
        }
        var itemOut = _mapper.Map<ShiftUserModel>(shiftUser);
        var users = await _context.Users.Where(x => !x.IsDelete && !x.Quit).Where(x => x.BranchId == user.BranchId).ToListAsync();
        itemOut.Items = await _context.ShiftUserDetails.Where(x => x.ShiftUserId == shiftUser.Id).Select(x => _mapper.Map<ShiftUserDetailModel>(x)).ToListAsync();
        itemOut.Items = itemOut.Items.ConvertAll(x =>
        {
            var user = users.FirstOrDefault(t => t.Id == x.UserId);
            x.UserFullName = user?.FullName;
            return x;
        }).Where(x => !string.IsNullOrEmpty(x.UserFullName)).ToList();
        return itemOut;
    }

    public async Task SyncUser(int id, int year)
    {
        var shiftUser = await _context.ShiftUsers.FindAsync(id);
        if (shiftUser is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var userIds = await _context.ShiftUserDetails.Where(x => x.ShiftUserId == shiftUser.Id).Select(x => x.UserId).ToListAsync();
        var users = await _context.Users.Where(x => !x.IsDelete && !x.Quit && !userIds.Contains(x.Id)).ToListAsync();

        if (!users.Any())
        {
            return;
        }

        var details = await GetUser(users, shiftUser.Month, year);
        var detailAdds = details.ConvertAll(x =>
        {
            var detailAdd = _mapper.Map<ShiftUserDetail>(x);
            detailAdd.ShiftUserId = shiftUser.Id;
            return detailAdd;
        });

        await _context.ShiftUserDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();
    }

    private async Task<List<ShiftUserDetailModel>> GetUser(IEnumerable<User> users, int month, int year)
    {
        var dayinMonth = DateTime.DaysInMonth(DateTime.Today.Year, month);
        // check holiday

        var workingday = await _context.WorkingDays.FirstOrDefaultAsync(x => x.Year == year);
        var listOut = new List<ShiftUserDetailModel>();
        foreach (var user in users)
        {

            var itemOut = new ShiftUserDetailModel()
            {
                UserId = user.Id,
                UserFullName = user.FullName,
                TargetId = user.TargetId,
            };
            for (int i = 1; i <= dayinMonth; i++)
            {
                var date = new DateTime(year, month, i);
                var dayworks = workingday.Days.Split(",").Select(x => x.ToLower());
                if (dayworks.Contains(date.ToString("dddd").ToLower()))
                {

                    PropertyInfo piInstance = itemOut.GetType().GetProperty($"Day{date.Day}");
                    piInstance.SetValue(itemOut, user.SymbolId);
                }
            }

            listOut.Add(itemOut);
        }
        return listOut;
    }
    public async Task SetShiftUserItem(int shiftUserId, ShiftUserDetailModel item)
    {
        var shiftUser = await _context.ShiftUsers.FindAsync(shiftUserId);
        if (shiftUser == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        // validate
        var shiftUserDetailCheck = await _context.ShiftUserDetails.AnyAsync(x => x.UserId == item.UserId && x.Id != item.Id && x.ShiftUserId == shiftUserId);
        if (shiftUserDetailCheck)
        {
            throw new ErrorException(ErrorMessages.DataExist);
        }

        if (item.IsDeleted)
        {
            var itemDel = await _context.ShiftUserDetails.FindAsync(item.Id);
            _context.ShiftUserDetails.Remove(itemDel);
            await _context.SaveChangesAsync();
            return;
        }

        var shiftUserDetail = _mapper.Map<ShiftUserDetail>(item);
        shiftUserDetail.ShiftUserId = shiftUserId;
        _context.ShiftUserDetails.Update(shiftUserDetail);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateShiftUserItems(int shiftUserId, List<ShiftUserDetailModel> shiftUserDetailModels)
    {
        var shiftUser = await _context.ShiftUsers.FindAsync(shiftUserId);
        if (shiftUser == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in shiftUserDetailModels)
            {
                var shiftUserDetail = await _context.ShiftUserDetails.FirstOrDefaultAsync(x =>
                                                    x.UserId == item.UserId
                                                    && x.Id != item.Id
                                                    && x.ShiftUserId == shiftUserId);
                if (shiftUserDetail != null)
                {
                    continue;
                }

                if (item.IsDeleted)
                {
                    var shiftUserDeleted = await _context.ShiftUserDetails.FindAsync(item.Id);
                    _context.ShiftUserDetails.Remove(shiftUserDeleted);
                }
                else
                {
                    var shiftUserDetailUpdate = _mapper.Map<ShiftUserDetail>(item);
                    shiftUserDetailUpdate.ShiftUserId = shiftUserId;
                    _context.ShiftUserDetails.Update(shiftUserDetailUpdate);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> UpdateShiftUsers(ShiftUserBodyRequestModel request)
    {
        var predicate = CreateShiftUsersPredicate(request);
        var shiftUsers = await _context.ShiftUsers
            .Where(predicate).ToListAsync();

        if (!shiftUsers.Any())
        {
            return false;
        }

        var shiftUserIds = shiftUsers.Select(s => s.Id).ToList();

        var details = await _context.ShiftUserDetails
            .Where(detail => shiftUserIds.Any(a => a == detail.ShiftUserId)
                             && request.UserIds.Contains(detail.UserId))
            .Join(_context.Users,
                detail => detail.UserId,
                user => user.Id, (shiftUserDetail, user) => shiftUserDetail)
            .ToListAsync();

        foreach (var shiftUser in shiftUsers)
        {
            foreach (var detail in details)
            {
                var detailType = detail.GetType();

                var dayProperties = detailType
                    .GetProperties()
                    .Where(p => p.Name.StartsWith("Day", StringComparison.CurrentCultureIgnoreCase))
                    .Select(s => new
                    {
                        Name = s.Name,
                        Day = int.Parse(s.Name.Replace("Day", ""))
                    });

                foreach (var propertyInfo in dayProperties)
                {
                    if (DateTime.TryParse($"{shiftUser.Year}/{shiftUser.Month}/{propertyInfo.Day}", out var datetime) && request.IsBetween(datetime))
                    {
                        detailType.GetProperty(propertyInfo.Name)?.SetValue(detail, request.Symbol);
                    }
                }

                _context.ShiftUserDetails.Update(detail);
            }
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<ShiftUserFilterResponseModel>> FilterShiftUser(ShiftUserFilterModel request)
    {
        var predicate = CreateShiftUsersPredicate(request);
        var shiftUsers = await _context.ShiftUsers
            .Where(predicate).ToListAsync();

        if (!shiftUsers.Any())
        {
            return new List<ShiftUserFilterResponseModel>();
        }

        var shiftUserIds = shiftUsers.Select(s => s.Id).ToList();

        var details = await _context.ShiftUserDetails
            .Where(detail => shiftUserIds.Any(a => a == detail.ShiftUserId))
            .Join(_context.Users,
            detail => detail.UserId,
            user => user.Id, (shiftUserDetail, user) => new
            {
                user,
                shiftUserDetail
            })
            .Skip(request.PageSize * (request.PageIndex - 1))
            .Take(request.PageSize)
            .ToListAsync();

        var response = new List<ShiftUserFilterResponseModel>();
        foreach (var shift in shiftUsers)
        {
            var item = new ShiftUserFilterResponseModel
            {
                Id = shift.Id,
                Name = shift.Name,
                FullName = $"{shift.Name}-{shift.Year}",
                Month = shift.Month,
                Year = shift.Year,
                Note = shift.Note,
            };

            var shiftDetail = details.Where(x => x.shiftUserDetail.ShiftUserId == shift.Id).ToList();
            foreach (var detail in shiftDetail)
            {
                var model = ConvertToShiftUserDetailResponse(detail.shiftUserDetail, shift, request);
                model.UserFullName = detail.user?.FullName;
                item.Details.Add(model);
            }

            response.Add(item);
        }
        return response;
    }

    private Expression<Func<ShiftUser, bool>> CreateShiftUsersPredicate(DateRangeFilter request)
    {
        var monthAndYears = request.GetMonthsAndYears().ToList();

        Expression<Func<ShiftUser, bool>> condition = null;
        foreach (var monthAndYear in monthAndYears)
        {
            Expression<Func<ShiftUser, bool>> predicate = x => x.Month == monthAndYear.Month && x.Year == monthAndYear.Year;
            condition = condition == null ? predicate : condition.Or(predicate);
        }

        return condition;
    }

    private ShiftUserDetailResponseModel ConvertToShiftUserDetailResponse(ShiftUserDetail detail, ShiftUser shift,
        ShiftUserFilterModel request)
    {
        var newModel = new ShiftUserDetailResponseModel
        {
            Id = detail.Id,
            UserId = detail.UserId,
            TargetId = detail.TargetId,
        };

        var dayProperties = detail.GetType()
            .GetProperties()
            .Where(p => p.Name.StartsWith("Day", StringComparison.CurrentCultureIgnoreCase))
            .Select(s => new
            {
                Name = s.Name,
                Value = s.GetValue(detail),
                Day = int.Parse(s.Name.Replace("Day", ""))
            });

        foreach (var propertyInfo in dayProperties)
        {
            if(DateTime.TryParse($"{shift.Year}/{shift.Month}/{propertyInfo.Day}", out var datetime) && request.IsBetween(datetime))
            {
                int.TryParse(propertyInfo.Value?.ToString(), out var schedule);
                newModel.UserSymbols.Add(new UserSymbol
                {
                    Date = datetime,
                    Symbol = schedule,
                });
            }
        }

        return newModel;
    }
}