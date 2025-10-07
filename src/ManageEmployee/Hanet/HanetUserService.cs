using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.HanetEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Hanet
{
    public class HanetUserService : IHanetUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public readonly IHanetRegister _hanetRegister;

        public HanetUserService(ApplicationDbContext context, IMapper mapper, IHanetRegister hanetRegister)
        {
            _context = context;
            _mapper = mapper;
            _hanetRegister = hanetRegister;
        }

        public async Task<PagingResult<HanetUserModel>> GetPaging(PagingRequestModel param)
        {
            var query = _context.HanetUsers
                    .Select(x => _mapper.Map<HanetUserModel>(x));

            return new PagingResult<HanetUserModel>
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
                TotalItems = await query.CountAsync()
            };
        }

        public async Task<HanetUserModel> GetDetail(int id)
        {
            return await _context.HanetUsers.Where(X => X.Id == id)
                    .Select(x => _mapper.Map<HanetUserModel>(x)).FirstOrDefaultAsync();
        }

        public async Task Set(HanetUserModel form)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var itemFind = await _context.HanetUsers.FindAsync(form.Id);
            if (itemFind is null)
            {
                //hanet regester
                await _hanetRegister.RegisterFace(form.UserIds, form.PlaceId);
            }
            else
            {
                // check user update
                var userIdStoreds = JsonConvert.DeserializeObject<List<int>>(itemFind.UserIds);
                // user remove
                var userIdRemoveds = userIdStoreds.Except(form.UserIds);
                if(userIdRemoveds.Any())
                {

                }

                // regester other user
                var userIdAdds = form.UserIds.Except(userIdStoreds);
                if (userIdAdds.Any())
                {
                    await _hanetRegister.RegisterFace(userIdAdds, form.PlaceId);
                }

                // update place
                var userIdUpdateds = form.UserIds.Where(x => userIdStoreds.Contains(x));
                if (itemFind.PlaceId != form.PlaceId && userIdUpdateds.Any())
                {

                }
            }
            var item = _mapper.Map<HanetUser>(form);
            _context.Update(item);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            var item = await _context.HanetUsers.FindAsync(id);
            _context.HanetUsers.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}