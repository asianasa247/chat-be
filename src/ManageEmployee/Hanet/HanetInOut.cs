using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.Enumerations.HanetEnums;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Hanet;
public class HanetInOut: IHanetInOut
{
    private readonly ApplicationDbContext _context;
    private readonly IInOutService _inOutService;

    public HanetInOut(IInOutService inOutService, ApplicationDbContext context)
    {
        _inOutService = inOutService;
        _context = context;
    }
    public async Task SetDataLogIn(HanetModel form)
    {
        if (form == null)
        {
            return;
        }
        if (form.data_type != DataTypeHanetConst.log)
        {
            return;
        }

        if (form.personType == PersonTypeHanet.Staff)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == form.personID);
            if (user is null)
                return;

            var shiftUser = await _context.ShiftUsers.FirstOrDefaultAsync(x => (x.Month == form.date.Month && x.Year == form.date.Year));
            if (shiftUser is null)
            {
                throw new ErrorException("Bạn chưa thiết lập ca");
            }

            var shiftUserDetail = await _context.ShiftUserDetails.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ShiftUserId == shiftUser.Id);
            if (shiftUserDetail == null)
            {
                return;
            }
            var symbolId = int.Parse(shiftUserDetail.GetType().GetProperty($"Day{form.date.Day}").GetValue(shiftUserDetail, null).ToString());
            var symbol = await _context.Symbols.FirstOrDefaultAsync(x => x.Id == symbolId);


            // insert data into inout
            var inout = new InOutHistory()
            {
                TimeIn = form.date,
                UserId = user.Id,
                SymbolId = symbol.Id,
                TargetId = user.TargetId ?? 0,
            };
            await _inOutService.Create(inout);
        }
        else
        {
            // insert data into customer
            var logging = new InOutLoggingFromHanet()
            {
                PersonType = form.personType,
                TimeIn = form.date,
            };
            await _context.InOutLoggingFromHanets.AddAsync(logging);
        }
        await _context.SaveChangesAsync();
    }
    public async Task SetDataLogOut(HanetModel form)
    {
        if (form == null)
        {
            return;
        }
        if (form.data_type != DataTypeHanetConst.log)
        {
            return;
        }

        if (form.personType == PersonTypeHanet.Staff)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == form.personID);
            if (user is null)
                return;


            // insert data into inout
            var inout = await _context.InOutHistories.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(inout is null)
            {
                return;
            }
            inout.TimeOut = form.date;
            await _inOutService.Update(inout);
        }
        
        await _context.SaveChangesAsync();
    }

}