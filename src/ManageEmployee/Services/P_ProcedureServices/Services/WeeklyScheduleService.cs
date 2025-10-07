using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WeeklyScheduleModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.ProcedureEntities.WeeklyScheduleEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices.Services
{
    public class WeeklyScheduleService : IWeeklyScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProcedureHelperService _procedureHelperService;

        public WeeklyScheduleService(ApplicationDbContext context,
            IMapper mapper,
            IProcedureHelperService procedureHelperService)
        {
            _context = context;
            _mapper = mapper;
            _procedureHelperService = procedureHelperService;
        }

        public async Task<PagingResult<WeeklySchedulePagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
        {
            var query = _context.WeeklySchedules
                        .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

            var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));

            if (param.StatusTab == ProduceProductStatusTab.Approved)
            {
                query = query
                .Join(_context.ProcedureLogs,
                        b => b.Id,
                        d => d.ProcedureId,
                        (b, d) => new
                        {
                            procedure = b,
                            log = d
                        })
                .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.WEEKLY_SCHEDULE) && x.log.UserId == userId && !x.procedure.IsFinished
                && x.log.NotAcceptCount == 0)
                .Select(x => x.procedure).Distinct();
            }

            if (param.StatusTab == ProduceProductStatusTab.Done)
            {
                query = query.Where(x => x.IsFinished);
            }
            query = query.QueryDate(param);
            query = query.QuerySearchTextProcedure(param);

            var datas = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
            var itemOuts = new List<WeeklySchedulePagingModel>();

            foreach (var item in datas)
            {
                var itemOut = _mapper.Map<WeeklySchedulePagingModel>(item);
                itemOut.UserName = await _context.Users.Where(x => x.Id == item.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
                itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                        && itemOut.ProcedureStatusName == startStatus.P_StatusName;
                itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;
                if (item.IsFinished)
                {
                    itemOut.ProcedureNumber = item.Code;
                }
                itemOuts.Add(itemOut);
            }
            var totalItem = await query.CountAsync();

            return new PagingResult<WeeklySchedulePagingModel>
            {
                Data = itemOuts,
                TotalItems = totalItem,
                PageSize = param.PageSize,
                CurrentPage = param.Page
            };
        }

        public async Task<WeeklyScheduleModel> GetDetail(int id, int userId)
        {
            var item = await _context.WeeklySchedules.FirstOrDefaultAsync(x => x.Id == id);
            var itemOut = _mapper.Map<WeeklyScheduleModel>(item);
            itemOut.Items = await _context.WeeklyScheduleDetails.Where(x => x.WeeklyScheduleId == id)
                            .Select(x => _mapper.Map<WeeklyScheduleDetailModel>(x)).ToListAsync();
            itemOut.IsSave = await CheckButton(id, userId);
            return itemOut;
        }

        public async Task Create(WeeklyScheduleModel form, int userId)
        {
            var procedure = _mapper.Map<WeeklySchedule>(form);
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            procedure.ProcedureStatusId = status.Id;
            procedure.ProcedureStatusName = status.P_StatusName;

            procedure.CreatedAt = DateTime.Now;
            procedure.UpdatedAt = DateTime.Now;
            procedure.UserCreated = userId;
            procedure.UserUpdated = userId;
            procedure.IsFinished = false;

            await _context.WeeklySchedules.AddAsync(procedure);
            await _context.SaveChangesAsync();

            var details = form.Items.Select(x => _mapper.Map<WeeklyScheduleDetail>(x)).ToList();
            details = details.ConvertAll(x =>
            {
                x.WeeklyScheduleId = procedure.Id;
                return x;
            });
            await _context.WeeklyScheduleDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.WEEKLY_SCHEDULE), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
        }

        public async Task Update(WeeklyScheduleModel form, int userId)
        {
            var procedure = await _context.WeeklySchedules.FindAsync(form.Id);
            if (procedure is null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }

            procedure.UpdatedAt = DateTime.Now;
            procedure.Note = form.Note;
            procedure.UserUpdated = userId;
            _context.WeeklySchedules.Update(procedure);

            var detailIds = form.Items.Select(X => X.Id).ToList();
            // delete item detail
            var detailDels = await _context.WeeklyScheduleDetails.Where(X => X.WeeklyScheduleId == form.Id && !detailIds.Contains(X.Id)).ToListAsync();
            if (detailDels.Any())
            {
                _context.WeeklyScheduleDetails.RemoveRange(detailDels);
            }

            var details = form.Items.Select(x => _mapper.Map<WeeklyScheduleDetail>(x)).ToList();
            details = details.ConvertAll(x =>
            {
                x.WeeklyScheduleId = procedure.Id;
                return x;
            });
            _context.WeeklyScheduleDetails.UpdateRange(details);
            await _context.SaveChangesAsync();
        }

        public async Task Accept(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var procedure = await _context.WeeklySchedules.FindAsync(id);
            procedure.UpdatedAt = DateTime.Now;
            procedure.UserUpdated = userId;
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
            if (status.ProcedureConditionCode == nameof(ProcedureOrderProduceProductConditionEnum.SameDepartment))
            {
                var userCreatedDepartmentId = await _context.Users.Where(x => x.Id == procedure.UserCreated).Select(x => x.DepartmentId).FirstOrDefaultAsync();
                var checkSameDepartment = await _context.Users.AnyAsync(x => x.Id == userId && x.DepartmentId == userCreatedDepartmentId);
                if (!checkSameDepartment)
                {
                    throw new ErrorException(ErrorMessages.ProcedureNotNotSameDepartment);
                }
            }
            procedure.ProcedureStatusId = status.Id;
            procedure.ProcedureStatusName = status.P_StatusName;

            if (status.IsFinish)
            {
                procedure.IsFinished = status.IsFinish;
                procedure.Code = await GetCodeAsync();
            }

            _context.WeeklySchedules.Update(procedure);
            await _context.SaveChangesAsync();

            await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.WEEKLY_SCHEDULE), status.Id, userId, id, procedure.ProcedureNumber);

            await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.WEEKLY_SCHEDULE), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);

            await transaction.CommitAsync();
        }

        public async Task NotAccept(int id, int userId)
        {
            var produce = await _context.WeeklySchedules.FindAsync(id);
            if (produce.IsFinished)
            {
                throw new ErrorException(ErrorMessages.ProcedureFinished);
            }

            produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
            // validate condition
            var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            produce.ProcedureStatusId = status.Id;
            produce.ProcedureStatusName = status.P_StatusName;

            produce.UpdatedAt = DateTime.Now;
            _context.WeeklySchedules.Update(produce);
            await _context.SaveChangesAsync();

            await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.WEEKLY_SCHEDULE), status.Id, userId, id, produce.ProcedureNumber, true);
        }

        public async Task Delete(int id)
        {
            var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            if (isExistLog)
            {
                throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
            }
            var item = await _context.WeeklySchedules.FindAsync(id);
            if (item != null)
            {
                _context.WeeklySchedules.Remove(item);
                await _context.SaveChangesAsync();
            }
            await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.WEEKLY_SCHEDULE));
        }

        public async Task<string> GetProcedureNumber()
        {
            var item = await _context.WeeklySchedules.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
            return $"{nameof(ProcedureEnum.WEEKLY_SCHEDULE)}-{procedureNumber}";
        }

        private async Task<string> GetCodeAsync()
        {
            var month = DateTime.Today.Month;

            var codeNumber = await _context.WeeklySchedules.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
            return _procedureHelperService.GetCode(codeNumber, "LV");
        }

        private async Task<bool> CheckButton(int id, int userId)
        {
            var procedure = await _context.WeeklySchedules.FindAsync(id);
            if (procedure is null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.WEEKLY_SCHEDULE));
            bool isSave = procedure.ProcedureStatusId == status.Id && procedure.UserCreated == userId;

            return isSave;
        }
    }
}